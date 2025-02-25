'use client'

import { useParamsStore } from "@/hooks/useParamsStore";
import { usePathname, useRouter } from "next/navigation";
import { BsBook } from "react-icons/bs";

export default function Logo() {
  const router = useRouter();
  const pathname = usePathname();

  function doReset(){
    if (pathname != "/"){
      router.push("/");
    }
    reset();
  }

  const reset = useParamsStore(state => state.reset);

  return (
    <div className='flex items-center gap-2 text-3xl font-semibold cursor-pointer' onClick={doReset}>
      <BsBook size={30}/>
      <div>Book store</div>
    </div>
  )
}
