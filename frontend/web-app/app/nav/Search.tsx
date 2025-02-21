'use client'

import { useParamsStore } from "@/hooks/useParamsStore";
import { FaSearch } from "react-icons/fa";

export default function Search() {
  const setParams = useParamsStore(state => state.setParams);
  const setSearchValue = useParamsStore(state => state.setSearchValue);
  const searchValue = useParamsStore(state => state.searchValue);

  function onChange(event: React.ChangeEvent<HTMLInputElement>){
    setSearchValue(event.target.value);
  }

  function search(){
    setParams({searchTerm: searchValue});
  }

  return (
    <div className="flex w-[50%] items-center border-2 rounded-full py-2 shadow-sm">
      <input
        onKeyDown={(e: React.KeyboardEvent<HTMLInputElement>) => {
          if (e.key == "Enter"){
            search();
          }
        }}
        onChange={onChange}
        value={searchValue}
        type="text" 
        placeholder="Search for books"
        className="flex-grow pl-5 bg-transparent focus:outline-none border-transparent 
          focus:border-transparent focus:ring-0 text-sm text-gray-600"
      />
      <button onClick={search}>
        <FaSearch 
          size={30}
          className="rounded-full bg-gray-600 text-white p-2 cursor-pointer mx-2"
        />
      </button>
    </div>
  )
}
