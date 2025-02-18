'use client'

import BookCard from "./BookCard";
import { Book } from "@/types/Book";
import AppPagination from "../components/AppPagination";
import { useEffect, useState } from "react";
import { getData } from "../actions/bookActions";
import Filters from "./Filters";
import { PagedResult } from "@/types/PagedResult";
import { useParamsStore } from "@/hooks/useParamsStore";
import { useShallow } from "zustand/shallow";
import queryString from "query-string";

export default function Listings() {
  const [data, setData] = useState<PagedResult<Book>>();
  const params = useParamsStore(useShallow(state => ({
    pageNumber: state.pageNumber,
    pageSize: state.pageSize,
    searchTerm: state.searchTerm,
    orderBy: state.orderBy,
    filterBy: state.filterBy
  })));
  const setParams = useParamsStore(state => state.setParams);
  const url = queryString.stringifyUrl({url: '', query: params})

  function setPageNumber(pageNumber: number){
    setParams({pageNumber})
  }

  useEffect(() => {
    getData(url).then(data => {
      setData(data)
    })
  }, [url])

  if (!data){
    return(
      <h3>Loading...</h3>
    )
  }

  return (
    <>
      <Filters />
      <div className="grid grid-cols-5 gap-6">
        {data.results.map((book) => (
          <BookCard book={book} key={book.id}/>
        ))}
      </div>
      <div className="flex justify-center mt-4">
        <AppPagination currentPage={params.pageNumber} pageCount={data.pageCount} pageChanged={setPageNumber} />
      </div>
    </>
  )
}
