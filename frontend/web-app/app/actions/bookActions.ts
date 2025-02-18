'use server'

import { Book } from "@/types/Book";
import { PagedResult } from "@/types/PagedResult";

export async function getData(query: string): Promise<PagedResult<Book>>{
    const res = await fetch(`http://localhost:6001/search${query}`, {
        next: { revalidate: 60 }
    });
    if (!res.ok){
        throw new Error("Failed to fetch data");
    }

    return res.json();
}