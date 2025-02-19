import EmptyFilter from "@/app/components/EmptyFilter";

export default function SignIn({ searchParams }: { searchParams: { callbackUrl: string } }) {
    return (
        <EmptyFilter
            title="Login to access this page"
            subtitle="Click below to login"
            showLogin
            callbackUrl={searchParams.callbackUrl}
        />
    )
}
