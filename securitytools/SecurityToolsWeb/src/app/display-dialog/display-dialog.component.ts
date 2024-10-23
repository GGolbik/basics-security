import { HttpEvent, HttpProgressEvent, HttpEventType, HttpErrorResponse } from '@angular/common/http';
import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Subject, Observable, tap, takeUntil } from 'rxjs';
import { ProblemDetails, ExecuteProgress, X509File } from '../tools/services/models';
import { SecurityToolsService } from '../tools/services/tool.service';

@Component({
  selector: 'app-display-dialog',
  templateUrl: './display-dialog.component.html',
  styleUrls: ['./display-dialog.component.css']
})
export class DisplayDialogComponent implements OnInit, OnDestroy {
  constructor(private service: SecurityToolsService,
    public dialogRef: MatDialogRef<DisplayDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DisplayDialogData,
  ) { }

  private ngUnsubscribe = new Subject<void>();

  protected loading = true;
  protected success = false;
  protected error?: ProblemDetails;

  protected uploadProgress?: ExecuteProgress;

  protected get progress() {
    return Math.round(this.uploadProgress?.progress ?? 0);
  }

  protected text: string = "";

  private getEventMessage(event: HttpEvent<any>) {
    // We are now getting events and can do whatever we want with them!
    let loaded = (event as HttpProgressEvent)?.loaded ?? 0;
    let total = (event as HttpProgressEvent)?.total ?? 0;
    let progress = total == 0 ? 0 : 100 / total * loaded;
    this.uploadProgress = { loaded: loaded, total: total, progress: progress } as ExecuteProgress;
  }

  ngOnInit(): void {
    let request: Observable<HttpEvent<string[]>>;
    if (this.data.file == null) {
      this.loading = false;
      this.error = {
        title: "No file",
        type: "No file",
        detail: "There is no file provided.",
        status: 400
      } as ProblemDetails;
      return;
    }
    let req: Observable<HttpEvent<string[]>>;
    if(this.data.realFile != null) {
      req = this.service.printFiles([this.data.realFile]);
    } else {
      req = this.service.print(this.data.file);
    }
    req.pipe(
      tap((event: HttpEvent<any>) => this.getEventMessage(event))
    ).pipe(
      takeUntil(this.ngUnsubscribe)
    ).subscribe({
      next: (response: HttpEvent<string[]>) => {
        if (response.type != HttpEventType.Response) {
          return;
        }
        this.loading = false;
        this.success = true;
        //let blob = base64ToArrayBuffer(cert.rawData ?? "");
        let blob = response.body;
        this.text = "";
        blob?.forEach((item) => {
          this.text += item;
        });
      },
      error: (err: any) => {
        this.loading = false;
        this.error = DisplayDialogComponent.toProblemDetails(err);
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
    if (!DisplayDialogComponent.isHttpErrorResponse(httpError)) {
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
    if (DisplayDialogComponent.isErrorResponse(httpError)) {
      let response = DisplayDialogComponent.getProblemDetails(httpError);
      return response.detail;
    } else if (DisplayDialogComponent.isHttpErrorResponse(httpError)) {
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
    if (DisplayDialogComponent.isErrorResponse(httpError)) {
      response = DisplayDialogComponent.getProblemDetails(httpError);
    } else if (DisplayDialogComponent.isHttpErrorResponse(httpError)) {
      let httpErrorResponse = httpError as HttpErrorResponse;
      response.title = '' + httpErrorResponse.status;
      response.detail = defaultText ?? httpErrorResponse.status + ': ' + httpErrorResponse.statusText;
    }
    return response;
  }

}

export interface DisplayDialogData {
  file?: X509File;
  realFile?: File;
}