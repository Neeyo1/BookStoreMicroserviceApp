import BookImage from "./BookImage"
import { Book } from "@/types/Book"

type Props = {
    book: Book
}

export default function BookCard({book}: Props) {
  return (
    <a href="#" className="group">
      <div className="relative w-full bg-gray-200 aspect-[2/3] rounded-lg overflow-hidden">
        <BookImage imageUrl={book.imageUrl} name={book.name}/>
      </div>
      <div className="flex justify-between items-center mt-4">
        <h3 className="text-gray-700">
          {book.name}
        </h3>
        <p className="font-semibold text-sm">
          {book.authorFirstName} {book.authorLastName}
        </p>
      </div>
    </a>
  )
}
