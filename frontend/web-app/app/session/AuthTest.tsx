'use client'

import { useState } from "react"
import { getAuthorsTest } from "../actions/bookActions";
import { Button } from "flowbite-react";

export default function AuthTest() {
    const [loading, setLoading] = useState(false);
    const [result, setResult] = useState<unknown>(null);

    function doGet(){
        setResult(undefined);
        setLoading(true);
        getAuthorsTest()
            .then(res => setResult(res))
            .catch(error => setResult(error))
            .finally(() => setLoading(false));
    }

    return (
        <div className="flex items-center gap-4">
            <Button outline isProcessing={loading} onClick={doGet}>
                Test auth
            </Button>
            <div>
                {JSON.stringify(result, null, 2)}
            </div>
        </div>
    )
}
