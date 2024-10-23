import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { WorkRequest, WorkStatus, WorkEventArgs, Dictionary } from "./models";


@Injectable({
    providedIn: 'root'
})
export class WorkerService {
    constructor(private http: HttpClient, @Inject('BASE_API_URL') private baseUrl: string) { }

    getRequests(): Observable<Dictionary<string, WorkRequest>> {
        return this.http.get<Dictionary<string, WorkRequest>>(this.baseUrl + "/worker/requests");
    }

    getStatus(limit?: number): Observable<Dictionary<string, WorkStatus>> {
        return this.http.get<Dictionary<string, WorkStatus>>(this.baseUrl + "/worker/status", {
            params: this.createParams(limit)
        });
    }

    getStatusOf(id: string): Observable<WorkStatus> {
        return this.http.get<WorkStatus>(this.baseUrl + "/worker/requests/" + id + "/status");
    }

    getResult(id: string): Observable<any> {
        return this.http.get<any>(this.baseUrl + "/worker/requests/" + id + "result");
    }

    getEvents(): Observable<WorkEventArgs[]> {
        return this.http.get<WorkEventArgs[]>(this.baseUrl + "/worker/events");
    }

    enqueue(kind: string, request: any) {
        return this.http.post<string>(this.baseUrl + "/worker/requests/" + kind, request);
    }

    cancel(id: string) {
        return this.http.delete<void>(this.baseUrl + "/worker/requests/" + id);
    }

    private createParams(limit?: number): HttpParams {
        let params = new HttpParams();
        if (limit != null) {
            params = params.append("limit", limit);
        }
        return params;
    }
}