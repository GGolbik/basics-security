import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Dictionary, KeyCredentials } from "./models";


@Injectable({
    providedIn: 'root'
})
export class CredentialsService {
    constructor(private http: HttpClient, @Inject('BASE_API_URL') private baseUrl: string) { }

    getCredentials(): Observable<Dictionary<string, KeyCredentials>> {
        return this.http.get<Dictionary<string, KeyCredentials>>(this.baseUrl + "credentials");
    }

    getCredentialsById(id: string): Observable<KeyCredentials> {
        return this.http.get<KeyCredentials>(this.baseUrl + "credentials/" + id);
    }

    addCredentials(credentials: KeyCredentials, password: string | undefined = undefined): Observable<string> {
        let formData = new FormData();
        formData.append('credentials', new Blob([JSON.stringify(credentials)], {
            type: "application/json",
        }));
        if (password != null)
            formData.append('password', password);
        return this.http.post<string>(this.baseUrl + "credentials", formData);
    }

    updateCredentials(id: string, credentials: KeyCredentials, password: string | undefined = undefined): Observable<void> {
        let formData = new FormData();
        formData.append('credentials', new Blob([JSON.stringify(credentials)], {
            type: "application/json",
        }));
        if (password != null)
            formData.append('password', password);
        return this.http.post<void>(this.baseUrl + "credentials/" + id, formData);
    }

    deleteCredentials(id: string): Observable<void> {
        return this.http.delete<void>(this.baseUrl + "credentials/" + id);
    }

    private createParams(password: string | undefined): HttpParams {
        let params = new HttpParams();
        if (password) {
            params = params.append("password", password);
        }
        return params;
    }
}