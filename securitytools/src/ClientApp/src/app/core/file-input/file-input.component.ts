import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DisplayDialogComponent, DisplayDialogData } from 'src/app/display-dialog/display-dialog.component';
import { ExecuteProgress, X509File } from 'src/app/services/models';
import { SecurityToolsService } from 'src/app/services/tool.service';
import { arrayBufferToBase64, renameFile, toHumanSize } from 'src/main';

@Component({
  selector: 'app-file-input',
  templateUrl: './file-input.component.html',
  styleUrls: ['./file-input.component.css']
})
export class FileInputComponent {
  protected toHumanSize = toHumanSize;

  protected progress?: ExecuteProgress;
  protected contentFile?: File;
  protected filename?: string;

  /**
   * Defines whether the user can change the input type.
   */
  @Input() hideRaw = false;
  /**
   * Defines whether the component is inputs and buttons are disabled.
   */
  @Input() disabled = false;
  /**
   * An optional password which is required to display the file.
   */
  @Input() password?: string;
  /**
   * Whether the raw file content shall be read as Base64 or as string.
   */
  @Input() readPlainText = false;
  /**
   * Whether only a button shall be visible to select a file.
   */
  @Input() hideInput = false;
  /**
   * The name of the input field.
   */
  @Input() name = "fileContent";
  /**
   * The file size limit for import of raw file content.
   */
  @Input() limit: number = 1024 * 1024 * 1; // 1 MiB
  /**
   * The label for the input field.
   */
  @Input() label?: string;

  private _isRaw = false;
  get isRaw() {
    return this._isRaw;
  }
  @Input() set isRaw(value) {
    this._isRaw = value;
    if (this.isRaw) {
      this.file = undefined;
    } else {
      this.content = undefined;
    }
  }

  private _file?: File;
  get file() {
    return this._file;
  }
  @Input() set file(value) {
    if (value != this._file) {
      this._file = value;
      this.filename = value?.name;
      if(this._file != null) {
        this._file = renameFile(this._file, Date.now() + "_" + this._file.name);
      }
      this.fileChange.emit(this._file);
    }
    if(value != null) {
      this.isRaw = false;
    }
  }
  @Output() fileChange = new EventEmitter<File>();

  private _content?: string;
  get content() {
    return this._content;
  }
  @Input() set content(value) {
    if(value == "" && this._content == null) {
      return;
    }
    if (value != this._content) {
      this._content = value;
      this.contentChange.emit(this._content);
    }
    if(value != null) {
      this.isRaw = true;
    }
  }
  @Output() contentChange = new EventEmitter<string | undefined>();

  constructor(private service: SecurityToolsService, private snackBar: MatSnackBar, private dialog: MatDialog) { }

  protected isValidBase64() {
    if (this.content == null || this.content == "") {
      return false;
    }
    try {
      atob(this.content);
      return true;
    } catch (err) {

    }
    return false;
  }

  protected onSelectFile(e: any) {
    this.file = (e.target.files[0] as File);
  }

  protected onSelectContent(e: any) {
    let contentFile = (e.target.files[0] as File);
    if (contentFile == null) {
      this.contentFile = undefined;
      this.content = undefined;
      return;
    }

    const reader = new FileReader();

    reader.onload = (e: any) => {
      if (this.readPlainText) {
        this.content = e.target.result;
      } else {
        this.content = arrayBufferToBase64(e.target.result);
      }
      this.contentChange.emit(this.content);
      this.progress = undefined;
    };
    reader.onerror = (e: any) => {
      console.error(e);
      this.progress = undefined;
      this.content = undefined;
      this.contentChange.emit(this.content);
    };
    reader.onprogress = (ev: ProgressEvent<FileReader>) => {
      let loaded = ev.loaded ?? 0;
      let total = ev.total ?? 0;
      let progress = total == 0 ? 0 : 100 / total * loaded;
      this.progress = { loaded: loaded, total: total, progress: progress } as ExecuteProgress;
    }

    if (contentFile.size > this.limit) {
      var humanFileSize = toHumanSize(contentFile.size);
      var humanLimit = toHumanSize(this.limit);
      let message = `File size of ${humanFileSize} exceeds the limit of ${humanLimit}.`;
      this.snackBar.open(message, 'Close', {
        duration: 3000,
        //verticalPosition: 'top'
      });
      return;
    }
    this.contentFile = contentFile;
    if (this.readPlainText) {
      reader.readAsText(this.contentFile);
    } else {
      reader.readAsArrayBuffer(this.contentFile);
    }
  }
  
  protected display() {
    const dialogRef = this.dialog.open(DisplayDialogComponent, {
      data: { file: { data: this._content, password: this.password } as X509File, realFile: this._file } as DisplayDialogData,
      closeOnNavigation: false,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      
    });
  }
}