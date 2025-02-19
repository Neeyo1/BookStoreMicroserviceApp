'use client'

import { Dropdown, DropdownDivider, DropdownItem } from "flowbite-react";
import { User } from "next-auth";
import { signOut } from "next-auth/react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { HiLogout } from "react-icons/hi";
import { HiCog, HiShoppingCart, HiUser } from "react-icons/hi2";

type Props = {
    user: User;
}

export default function UserActions({user}: Props) {
    const router = useRouter();
    return (
        <Dropdown inline label={`Welcome ${user.name}`}>
            <DropdownItem icon={HiUser}>
                <Link href="/">
                    Account
                </Link>
            </DropdownItem>
            <DropdownItem icon={HiShoppingCart}>
                <Link href="/">
                    My cart
                </Link>
            </DropdownItem>
            <DropdownItem icon={HiCog}>
                <Link href="/session">
                    Session
                </Link>
            </DropdownItem>
            <DropdownDivider />
            <DropdownItem icon={HiLogout} onClick={() => signOut({callbackUrl: "/"})}>
                Sign out
            </DropdownItem>
        </Dropdown>
    )
}
