import { TableContinuationToken } from './TableContinuationToken';
export class PagedQueryResult<T> {
    resultPage: T[];
    continuationToken: string;
}
