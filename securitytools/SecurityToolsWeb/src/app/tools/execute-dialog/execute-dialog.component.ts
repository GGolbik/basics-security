import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ConfigCert, ConfigCrl, ConfigCsr, ConfigKeyPair, ConfigTransform, ExecuteProgress, MediaType, ProblemDetails } from '../services/models';
import { Observable, Subject, takeUntil, tap, timer } from 'rxjs';
import { HttpErrorResponse, HttpEvent, HttpEventType, HttpProgressEvent } from '@angular/common/http';
import { SecurityToolsService } from '../services/tool.service';

@Component({
  selector: 'app-execute-dialog',
  templateUrl: './execute-dialog.component.html',
  styleUrls: ['./execute-dialog.component.css']
})
export class ExecuteDialogComponent implements OnInit, OnDestroy {
  constructor(private service: SecurityToolsService,
    public dialogRef: MatDialogRef<ExecuteDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ExecuteDialogData,
  ) { }

  private ngUnsubscribe = new Subject<void>();

  loading = true;
  success = false;
  error?: ProblemDetails;

  uploadProgress?: ExecuteProgress;

  private getEventMessage(event: HttpEvent<any>) {
    // We are now getting events and can do whatever we want with them!
    let loaded = (event as HttpProgressEvent)?.loaded ?? 0;
    let total = (event as HttpProgressEvent)?.total ?? 0;
    let progress = total == 0 ? 0 : 100 / total * loaded;
    this.uploadProgress = { loaded: loaded, total: total, progress: progress } as ExecuteProgress;
  }

  ngOnInit(): void {
    let request: Observable<HttpEvent<ArrayBuffer>>;
    if (this.data.cert != null) {
      request = this.service.buildCert(this.data.cert, [], this.data.accepts);
    } else if (this.data.crl != null) {
      request = this.service.buildCrl(this.data.crl, [], this.data.accepts);
    } else if (this.data.csr != null) {
      request = this.service.buildCsr(this.data.csr, [], this.data.accepts);
    } else if (this.data.keyPair != null) {
      request = this.service.buildKeyPair(this.data.keyPair, [], this.data.accepts);
    } else if (this.data.transform != null) {
      request = this.service.buildTransform(this.data.transform, [], this.data.accepts);
    } else {
      this.loading = false;
      this.error = {
        title: "Implementation Error",
        type: "Implementation Error",
        detail: "There is no config provided.",
        status: 400
      } as ProblemDetails;
      return;
    }
    request.pipe(
      tap((event: HttpEvent<any>) => this.getEventMessage(event))
    ).pipe(
      takeUntil(this.ngUnsubscribe)
    ).subscribe({
      next: (response: HttpEvent<ArrayBuffer>) => {
        if (response.type != HttpEventType.Response) {
          return;
        }
        // attachment; filename=result.zip; filename*=UTF-8''result.zip
        let filename = response.headers.get('content-disposition');
        if (filename != null) {
          let regex = /filename=(?<name>.*);/.exec(filename);
          if (regex?.groups != null) {
            filename = regex?.groups['name'];
            if (filename.startsWith('"')) {
              filename = filename.substring(1);
            }
            if (filename.endsWith('"')) {
              filename = filename.substring(0, filename.length - 1);
            }
            if (filename.trim() == "") {
              filename = null;
            }
          } else {
            filename = null;
          }
        }
        this.loading = false;
        this.success = true;
        var a = document.createElement("a");
        //let blob = base64ToArrayBuffer(cert.rawData ?? "");
        let blob = response.body!;
        switch (this.data.accepts) {
          case MediaType.Zip:
            a.href = URL.createObjectURL(new Blob([blob], { type: "application/zip" }));
            filename ??= "result.zip";
            break;
          case MediaType.Tar:
            a.href = URL.createObjectURL(new Blob([blob], { type: "application/x-tar" }));
            filename ??= "result.tar";
            break;
          case MediaType.TarGz:
            a.href = URL.createObjectURL(new Blob([blob], { type: "application/gzip" }));
            filename ??= "result.tar.gz";
            break;
          default:
            a.href = URL.createObjectURL(new Blob([blob], { type: "application/octet-stream" }));
            filename ??= "result";
            break;
        }
        filename ??= "result";
        a.download = filename;
        a.click();
        URL.revokeObjectURL(a.href);
        timer(2000).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
          next: () => {
            this.onNoClick();
          }
        })
      },
      error: (err: any) => {
        this.loading = false;
        this.error = ExecuteDialogComponent.toProblemDetails(err);
      },
      complete: () => { },
    });
  }

  ngOnDestroy() {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  onNoClick(): void {
    this.dialogRef.close(this.success ? true : (this.error ?? false));
  }

  public static isHttpErrorResponse(httpError?: any) {
    if (httpError == null) {
      return false;
    }
    return httpError instanceof HttpErrorResponse;
  }

  public static isErrorResponse(httpError?: any): boolean {
    if (!ExecuteDialogComponent.isHttpErrorResponse(httpError)) {
      return false;
    }
    let httpErrorResponse = httpError as HttpErrorResponse;
    if (httpErrorResponse.error == null) {
      return false;
    }
    try {
      let jsonResult = httpErrorResponse.error instanceof ArrayBuffer ? JSON.parse(new TextDecoder().decode(httpErrorResponse.error)) : httpErrorResponse.error;
      if (!jsonResult.hasOwnProperty('type')) {
        return false;
      }
      if (!jsonResult.hasOwnProperty('title')) {
        return false;
      }
      return true;
    } catch (e) {
      return false;
    }
  }

  static getProblemDetails(httpError?: any): ProblemDetails {
    let httpErrorResponse = httpError as HttpErrorResponse;
    let jsonResult: ProblemDetails;
    if (httpErrorResponse.error instanceof ArrayBuffer) {
      jsonResult = JSON.parse(new TextDecoder().decode(httpErrorResponse.error));
      //jsonResult.detail = jsonResult.title;
      //jsonResult.title = jsonResult.status + '';
      //jsonResult.type = "doc/errors#" + jsonResult.status
    } else {
      jsonResult = httpErrorResponse.error;
    }
    return jsonResult as ProblemDetails;
  }

  /**
   * Returns an error text.
   * @param httpError The http error response.
   * @param defaultIfHttpError the text to return if is not ErrorResponse.
   * @param defaultText the text to return if is not HttpErrorResponse.
   * @returns The error text.
   */
  public static getErrorText(httpError?: any, defaultIfHttpError?: string, defaultText?: string) {
    if (ExecuteDialogComponent.isErrorResponse(httpError)) {
      let response = ExecuteDialogComponent.getProblemDetails(httpError);
      return response.detail;
    } else if (ExecuteDialogComponent.isHttpErrorResponse(httpError)) {
      let response = httpError as HttpErrorResponse;
      return response.status + ': ' + response.statusText;
    } else {
      return defaultText ?? "Unknown error";
    }
  }

  /**
   * Opens an error snackBar.
   * @param snackBar The MatSnackBar object to use.
   * @param httpError The http error
   * @param defaultText The text to show if the http error is not of type ErrorResponse
   * @param duration The time span until the snackbar is closed.
   * @param showClose Whether the close button shall be shown.
   */
  static toProblemDetails(httpError?: any, defaultText?: string, duration?: number, showClose: boolean = true): ProblemDetails {
    console.log(httpError);
    let response = { type: '', title: 'Unknown error', details: defaultText ?? '' } as ProblemDetails;
    if (ExecuteDialogComponent.isErrorResponse(httpError)) {
      response = ExecuteDialogComponent.getProblemDetails(httpError);
    } else if (ExecuteDialogComponent.isHttpErrorResponse(httpError)) {
      let httpErrorResponse = httpError as HttpErrorResponse;
      response.title = '' + httpErrorResponse.status;
      response.detail = defaultText ?? httpErrorResponse.status + ': ' + httpErrorResponse.statusText;
    }
    return response;
  }

}

export interface ExecuteDialogData {
  accepts: MediaType;
  cert?: ConfigCert;
  csr?: ConfigCsr;
  crl?: ConfigCrl;
  transform?: ConfigTransform;
  keyPair?: ConfigKeyPair;
}