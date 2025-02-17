import React from 'react'
import { BsBook } from "react-icons/bs";

export default function Navbar() {
  return (
    <header className='sticky top-0 z-50 flex justify-between bg-white p-5 text-gray-800 shadow-md'>
      <div className='flex items-center gap-2 text-3xl font-semibold'>
        <BsBook size={30}/>
        <div>Book store</div>
      </div>
      <div>Search</div>
      <div>Login</div>
    </header>
  )
}
