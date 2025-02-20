'use server'

import { auth } from "@/auth";
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

export async function getAuthorsTest(){
    const session = await auth();

    const res = await fetch("http://localhost:6001/authors", {
        method: "GET",
        headers: {
            "Content-type": "application/json",
            "Authorization": `Bearer ${session?.accessToken}`
        },
        next: { revalidate: 60 }
    });

    if (!res.ok){
        return {status: res.status, message: res.statusText};
    }

    return res.status;
}