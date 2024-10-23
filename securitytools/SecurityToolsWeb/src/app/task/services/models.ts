
export type DateTime = string;
export type Dictionary<K extends string | number | symbol, V> = Record<K, V>;

export enum WorkState {
    None = 0,
    Waiting = 1,
    Executing = 2,
    Error = 3,
    Done = 4
}

export interface WorkRequest {
    kind: string;
    notBefore: string;
    timeout: string;
}

export interface WorkStatus {
    state: WorkState;
    enqueued?: DateTime;
    executionStart?: DateTime;
    executionEnd?: DateTime;
    executionDuration?: string;
    error?: string;
}

export interface WorkEventArgs {
    id: string;
    kind: string;
    status: WorkStatus;
}