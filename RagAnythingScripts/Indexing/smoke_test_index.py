#!/usr/bin/env python3
"""
Smoke test for rag-anything indexing using Python API.
Recursively indexes known text files from an input folder into a rag-anything database.
"""
import os
import sys
import json
import asyncio
import argparse
from pathlib import Path
from argparse import Namespace
from raganything import RAGAnything, RAGAnythingConfig
from lightrag.utils import EmbeddingFunc
from lightrag.llm.openai import openai_complete_if_cache

# Ensure examples_repo is in path for local raganything import if not globally installed
repo_path = str(Path(__file__).parents[2] / "examples_repo")
if repo_path not in sys.path:
    sys.path.insert(0, repo_path)

# Supported text extensions for smoke test
TEXT_EXTENSIONS = {
    ".txt", ".md", ".json", ".xml", ".csv", ".log", ".yaml", ".yml",
    ".toml", ".ini", ".cfg", ".conf", ".rst",
    ".html", ".htm",
    ".py", ".js", ".ts", ".java", ".c", ".cpp", ".h", ".hpp", ".cs",
    ".php", ".rb", ".go", ".rs", ".sh", ".bat", ".ps1", ".sql"
}

async def main(input_folder: str, output_folder: str):
    script_dir = Path(__file__).parent
    config_path = script_dir / "config.json"
    
    if not config_path.exists():
        print(f"❌ Config file not found: {config_path}")
        sys.exit(1)
        
    with open(config_path, "r") as f:
        config = json.load(f)
        
    ollama_host = config.get("ollama_host", "http://localhost:11434")
    llm_model = config.get("llm_model", "llama3.2")
    embed_model = config.get("embedding_model", "nomic-embed-text")
    embed_dim = int(config.get("embedding_dim", 768))
    
    # Ollama LLM wrapper
    async def ollama_llm(prompt, system_prompt=None, history_messages=None, **kwargs):
        return await openai_complete_if_cache(
            model=llm_model,
            prompt=prompt,
            system_prompt=system_prompt,
            history_messages=history_messages or [],
            base_url=f"{ollama_host}/v1",
            api_key="ollama",
            **kwargs
        )

    # Ollama Embedding wrapper
    async def ollama_embed(texts):
        import ollama
        client = ollama.AsyncClient(host=ollama_host)
        resp = await client.embed(model=embed_model, input=texts)
        return resp.embeddings

    # Initialize RAG-Anything
    rag_cfg = RAGAnythingConfig(
        working_dir=output_folder,
        parser="txt",
        parse_method="txt",
        enable_image_processing=False,
        enable_table_processing=False,
        enable_equation_processing=False,
    )

    embed_func = EmbeddingFunc(
        embedding_dim=embed_dim,
        max_token_size=8192,
        func=ollama_embed
    )

    print(f"🚀 Initializing rag-anything (Ollama: {ollama_host})...")
    rag = RAGAnything(
        config=rag_cfg,
        llm_model_func=ollama_llm,
        embedding_func=embed_func,
    )

    # Gather text files recursively
    files_to_index = []
    for root, _, filenames in os.walk(input_folder):
        for fn in filenames:
            if Path(fn).suffix.lower() in TEXT_EXTENSIONS:
                files_to_index.append(os.path.join(root, fn))

    if not files_to_index:
        print("ℹ️  No supported text files found.")
        return

    print(f"📂 Found {len(files_to_index)} text files to index.")
    
    success_count = 0
    for i, fpath in enumerate(files_to_index, 1):
        print(f"[{i}/{len(files_to_index)}] Processing: {fpath}")
        try:
            await rag.process_document_complete(file_path=fpath, output_dir=output_folder)
            success_count += 1
            print(f"   ✅ Success")
        except Exception as e:
            print(f"   ❌ Failed: {e}")

    print(f"\n🎉 Indexing complete. {success_count}/{len(files_to_index)} files processed.")

if __name__ == "__main__":
    # parser = argparse.ArgumentParser(description="rag-anything smoke test indexer")
    # parser.add_argument("input_folder", help="Directory containing files to index")
    # parser.add_argument("output_folder", help="Directory for RAG output/database")
    # args = parser.parse_args()
    # args = {
    #     input_folder = "",
    #     output_folder = ""
    # }
    args = Namespace(
        input_folder=r"D:\LocalFolderKnowledge\copy test\copy",
        output_folder=r"D:\LocalFolderKnowledge\copy test\rag-anything"
    )

    if not os.path.isdir(args.input_folder):
        print(f"❌ Input folder not found: {args.input_folder}")
        sys.exit(1)

    os.makedirs(args.output_folder, exist_ok=True)
    asyncio.run(main(args.input_folder, args.output_folder))
