import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatStepper } from '@angular/material/stepper';
import { Router, ActivatedRoute } from '@angular/router';
import { ExecuteDialogComponent, ExecuteDialogData } from '../../execute-dialog/execute-dialog.component';
import { MediaType, X509Encoding, ProblemDetails, FragmentParams, ConfigKeyPair } from '../../../services/models';
import { timer } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';
import { exportJson } from '../../../../main';

@Component({
  selector: 'app-key-pair',
  templateUrl: './key-pair.component.html',
  styleUrls: ['./key-pair.component.css']
})
export class KeyPairComponent implements OnInit {
  public MediaType = MediaType;
  public X509Encoding = X509Encoding;
  public Number = Number;

  accepts = MediaType.Default;
  fileFormat? = X509Encoding.Default;

  @ViewChild('stepper') stepper?: MatStepper;
  isLinear = false;
  error?: ProblemDetails;

  params = new FragmentParams();
  completedDone = false;
  completedGetStarted = true;
  get completedKeyPair() {
    return this.config != null;
  }

  constructor(private router: Router, private route: ActivatedRoute, public dialog: MatDialog, private snackBar: MatSnackBar) {
  }

  ngOnInit(): void {
    this.params = FragmentParams.parse(this.route);
  }

  selectedIndexChange(value: any) {
    //this.params.stepperIndex = value.selectedIndex;
    //this.params.updateUrl(this.router);
  }

  config?: ConfigKeyPair;

  create() {
    this.error = undefined;
    let config = this.config ?? {};
    if (this.fileFormat != null) {
      config.privateKey ??= {};
      config.privateKey.fileFormat ??= {};
      if ((config.privateKey.fileFormat.encoding ?? X509Encoding.Default) == X509Encoding.Default) {
        config.privateKey.fileFormat.encoding = this.fileFormat;
      }
      config.publicKey ??= {};
      config.publicKey.fileFormat ??= {};
      config.publicKey.fileFormat.encoding = this.fileFormat;
    }
    const dialogRef = this.dialog.open(ExecuteDialogComponent, {
      data: { accepts: this.accepts, keyPair: config } as ExecuteDialogData,
      closeOnNavigation: false,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.completedDone = true;
      } else if (result != null && 'type' in result && 'title' in result) {
        this.error = result as ProblemDetails;
      }
    });
  }

  reset() {
    this.completedGetStarted = false;
    this.completedDone = false;
    this.config = undefined;
    this.stepper?.reset();
    timer(0).subscribe({
      next: () => {
        this.completedGetStarted = true;
      }
    })
  }

  import(content: any) {
    try {
      let result = JSON.parse(content);
      this.reset();
      this.config = result ?? {};
      this.stepper?.next();
    } catch (err) {
      let message = "Failed to parse content to JSON. " + err;
      this.snackBar.open(message, 'Close', {
        //duration: 10000,
        //verticalPosition: 'top'
      });
    }
  }

  export() {
    exportJson(this.config);
  }
}
