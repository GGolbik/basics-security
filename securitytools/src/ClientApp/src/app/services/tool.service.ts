import { Inject, Injectable } from '@angular/core';
import { HttpClient, HttpEvent, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConfigCert, ConfigCrl, ConfigCsr, ConfigKeyPair, ConfigTransform, Dictionary, MediaType, ProgramInfo, X509File } from './models';

@Injectable({
    providedIn: 'root'
})
export class SecurityToolsService {

    constructor(private http: HttpClient, @Inject('BASE_API_URL') private baseUrl: string) { }

    getInfo() {
        return this.http.get<ProgramInfo>(this.baseUrl + "securitytools/info");
    }

    buildCert(config: ConfigCert, files: File[] = [], accepts = MediaType.Zip): Observable<HttpEvent<ArrayBuffer>> {
        let request = new ConfigRequest();
        request.config = config;
        request.files = files;
        return this.build(this.baseUrl + "securitytools/build/certificate", request.createFormData(), accepts);
    }

    buildCrl(config: ConfigCrl, files: File[] = [], accepts = MediaType.Zip): Observable<HttpEvent<ArrayBuffer>> {
        let request = new ConfigRequest();
        request.config = config;
        request.files = files;
        return this.build(this.baseUrl + "securitytools/build/crl", request.createFormData(), accepts);
    }

    buildCsr(config: ConfigCsr, files: File[] = [], accepts = MediaType.Zip): Observable<HttpEvent<ArrayBuffer>> {
        let request = new ConfigRequest();
        request.config = config;
        request.files = files;
        return this.build(this.baseUrl + "securitytools/build/csr", request.createFormData(), accepts);
    }

    buildKeyPair(config: ConfigKeyPair, files: File[] = [], accepts = MediaType.Zip): Observable<HttpEvent<ArrayBuffer>> {
        let request = new ConfigRequest();
        request.config = config;
        request.files = files;
        return this.build(this.baseUrl + "securitytools/build/keypair", request.createFormData(), accepts);
    }

    buildTransform(config: ConfigTransform, files: File[] = [], accepts = MediaType.Zip): Observable<HttpEvent<ArrayBuffer>> {
        let request = new ConfigRequest();
        request.config = config;
        request.files = files;
        return this.build(this.baseUrl + "securitytools/build/transform", request.createFormData(), accepts);
    }

    print(file: X509File): Observable<HttpEvent<string[]>> {
        return this.http.post<string[]>(this.baseUrl + "securitytools/print", file, {
            responseType: "json",
            reportProgress: true,
            observe: 'events',
        });
    }

    printFiles(files: File[] = []): Observable<HttpEvent<string[]>> {
        let request = new ConfigRequest();
        request.files = files;
        return this.http.post<string[]>(this.baseUrl + "securitytools/print/files", request.createFormData(), {
            responseType: "json",
            reportProgress: true,
            observe: 'events',
        });
    }

    private build(apiUrl: string, request: any, accepts: MediaType) {
        let headers = new HttpHeaders();
        switch (accepts) {
            case MediaType.Zip:
                headers = new HttpHeaders({ Accept: 'application/zip' });
                break;
            case MediaType.Tar:
                headers = new HttpHeaders({ Accept: 'application/x-tar' });
                break;
            case MediaType.TarGz:
                headers = new HttpHeaders({ Accept: 'application/gzip' });
                break;
            default:
                headers = new HttpHeaders({ Accept: 'application/octet-stream' });
                break;
        }

        return this.http.post(apiUrl, request, {
            responseType: "arraybuffer",
            headers: headers,
            reportProgress: true,
            observe: 'events',
        });
    }
}

class ConfigRequest {
    config?: ConfigCert | ConfigCsr | ConfigCrl | ConfigTransform | ConfigKeyPair;
    files?: File[];

    createFormData(): FormData {
        let formData = new FormData();
        if (this.config != null) {
            formData.append('config', new Blob([JSON.stringify(this.config)], {
                type: "application/json"
            }));
        }

        if (this.files != null) {
            this.files.forEach((file) => {
                formData.append('files[]', file);
            });
        }

        return formData;
    }
}