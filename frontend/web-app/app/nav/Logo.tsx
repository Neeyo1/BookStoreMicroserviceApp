'use client'

import { useParamsStore } from "@/hooks/useParamsStore";
import { BsBook } from "react-icons/bs";

export default function Logo() {
  const reset = useParamsStore(state => state.reset);

  return (
    <div className='flex items-center gap-2 text-3xl font-semibold cursor-pointer' onClick={reset}>
      <BsBook size={30}/>
      <div>Book store</div>
    </div>
  )
}
