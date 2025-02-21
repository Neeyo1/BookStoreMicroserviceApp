import React from "react";
import EmptyFilter from "@/app/components/EmptyFilter";

export default async function SignIn({ searchParams }: { searchParams: Promise<{ callbackUrl: string }> }) {
    const params = await searchParams;

    return (
        <EmptyFilter
            title="Login to access this page"
            subtitle="Click below to login"
            showLogin
            callbackUrl={params.callbackUrl}
        />
    );
}
