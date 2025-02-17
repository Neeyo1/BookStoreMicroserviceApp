export type Book = {
    name: string;
    year: number;
    imageUrl: string;
    price: number;
    items: number;
    authorId: string;
    authorFirstName?: string;
    authorLastName?: string;
    authorAlias?: string;
    publisherId: string;
    publisherName: string;
    publisherCountry: string;
    publisherCity: string;
    publisherAddress: string;
    publisherPhoneNumber: string;
    publisherPageUrl?: string;
    id: string;
}