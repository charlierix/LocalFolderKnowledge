#!/usr/bin/env python3
"""
Smoke test for rag-anything querying using Python API.
Performs a simple text query against a rag-anything database.
"""
import os
import sys
import json
import asyncio
import nest_asyncio
from pathlib import Path
from argparse import Namespace
from raganything import RAGAnything, RAGAnythingConfig
from lightrag.utils import EmbeddingFunc
from lightrag.llm.openai import openai_complete_if_cache

async def main(rag_folder: str):
    script_dir = Path(__file__).parent
    config_path = script_dir.parent / "config.json"
    
    if not config_path.exists():
        print(f"❌ Config file not found: {config_path}")
        sys.exit(1)
        
    with open(config_path, "r") as f:
        config = json.load(f)
        
    ollama_host_llm = config.get("ollama_host_llm")
    ollama_host_embed = config.get("ollama_host_embed")
    llm_model = config.get("llm_model")
    embed_model = config.get("embedding_model")
    embed_dim = int(config.get("embedding_dim"))
    
    # Ollama LLM wrapper
    async def ollama_llm(prompt, system_prompt=None, history_messages=None, **kwargs):
        return await openai_complete_if_cache(
            model=llm_model,
            prompt=prompt,
            system_prompt=system_prompt,
            history_messages=history_messages or [],
            base_url=f"{ollama_host_llm}/v1",
            api_key="ollama",
            **kwargs
        )
    
    # Ollama Embedding wrapper
    async def ollama_embed(texts):
        import ollama
        client = ollama.AsyncClient(host=ollama_host_embed)
        resp = await client.embed(model=embed_model, input=texts)
        return resp.embeddings
    
    # Initialize RAG-Anything
    rag_cfg = RAGAnythingConfig(
        working_dir=rag_folder,
        parser="mineru",
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
    
    print(f"🚀 Initializing rag-anything (Ollama llm: {ollama_host_llm}, Ollama embed: {ollama_host_embed})...")
    rag = RAGAnything(
        config=rag_cfg,
        llm_model_func=ollama_llm,
        embedding_func=embed_func,
    )
    
    # Perform a simple text query
    query_text = "What is the main topic of this document?"
    print(f"🔍 Asking: {query_text}")
    
    try:
        result = await rag.query(query_text)
        print(f"\n✅ Query result:\n{result}")
    except Exception as e:
        print(f"❌ Query failed: {e}")
        sys.exit(1)

if __name__ == "__main__":
    # Apply nest_asyncio to allow nested event loops
    nest_asyncio.apply()
    
    args = Namespace(
        rag_folder=r"D:\LocalFolderKnowledge\copy test\rag-anything"
    )
    
    if not os.path.isdir(args.rag_folder):
        print(f"❌ RAG folder not found: {args.rag_folder}")
        sys.exit(1)
    
    asyncio.run(main(args.rag_folder))
