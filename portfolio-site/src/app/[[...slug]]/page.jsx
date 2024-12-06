import '../../index.css';
import { ClientOnly } from './client';

export function generateStaticParams(){
    return [{params: {slug: ['']}}]
}

export default function Page(){
    return <ClientOnly />
}