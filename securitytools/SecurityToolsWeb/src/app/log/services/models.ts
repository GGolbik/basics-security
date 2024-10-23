
export type DateTime = string;

export enum LogLevel {
    Trace = 0,
    Debug = 1,
    Information = 2,
    Warning = 3,
    Error = 4,
    Critical = 5,
    None = 6
}

export interface LogConfig {
    level: LogLevel
}

export interface LogEntry {
    schemaVersion?: string;
    id?: number;
    timestamp?: DateTime;
    level?: LogLevel;
    exception?: string;
    message?: string;
    properties?: string;
}

export enum SortDirection {
    Ascending = 0,
    Descending = 1
}

export interface LogFilterOptions {
    limit?: number;
    offset?: number;
    sorting?: SortDirection;
    search?: string;
    level?: LogLevel;
    since?: DateTime;
    until?: DateTime;
}
