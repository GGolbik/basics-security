import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { LogEntry, LogFilterOptions, LogConfig } from "./models";


@Injectable({
    providedIn: 'root'
})
export class LoggingService {
    constructor(private http: HttpClient, @Inject('BASE_API_URL') private baseUrl: string) { }

    getConfig(): Observable<LogConfig> {
        return this.http.get<LogConfig>(this.baseUrl + "log/config");
    }

    setConfig(config: LogConfig) : Observable<any> {
        return this.http.put(this.baseUrl + "log/config", config);
    }

    deleteEntries() : Observable<any> {
        return this.http.delete(this.baseUrl + "log/entries");
    }

    getEntries(options: LogFilterOptions): Observable<LogEntry[]> {
        let params = this.createParams(options);
        return this.http.get<LogEntry[]>(this.baseUrl + "log/entries", {
            params: params
        });
    }

    private createParams(options: LogFilterOptions): HttpParams {
        let params = new HttpParams();
        if (options.limit != null) {
            params = params.append("limit", options.limit);
        }
        if (options.offset != null) {
            params = params.append("offset", options.offset);
        }
        if (options.sorting != null) {
            params = params.append("sorting", options.sorting);
        }
        if (options.search != null) {
            params = params.append("search", options.search);
        }
        if (options.level != null) {
            params = params.append("level", options.level);
        }
        if (options.since != null) {
            params = params.append("since", options.since);
        }
        if (options.until != null) {
            params = params.append("until", options.until);
        }
        return params;
    }
}