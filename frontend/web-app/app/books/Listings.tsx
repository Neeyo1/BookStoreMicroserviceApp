import { PagedResult } from "@/types/PagedResult";
import BookCard from "./BookCard";
import { Book } from "@/types/Book";

async function getData(): Promise<PagedResult<Book>>{
    const res = await fetch("http://localhost:6001/search", {
        next: { revalidate: 60 }
    });
    if (!res.ok){
        throw new Error("Failed to fetch data");
    }

    return res.json();
}

export default async function Listings() {
  const data = await getData();
  return (
    <div className="grid grid-cols-5 gap-6">
      {data && data.results.map((book) => (
        <BookCard book={book} key={book.id}/>
      ))}
    </div>
  )
}
