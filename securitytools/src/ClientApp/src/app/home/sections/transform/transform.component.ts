import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatStepper } from '@angular/material/stepper';
import { Router, ActivatedRoute } from '@angular/router';
import { timer } from 'rxjs';
import { ExecuteDialogComponent, ExecuteDialogData } from 'src/app/home/execute-dialog/execute-dialog.component';
import { MediaType, X509Encoding, ProblemDetails, FragmentParams, X509File, ConfigTransform, TransformMode } from 'src/app/services/models';
import { exportJson } from 'src/main';

@Component({
  selector: 'app-transform',
  templateUrl: './transform.component.html',
  styleUrls: ['./transform.component.css']
})
export class TransformComponent implements OnInit {
  public MediaType = MediaType;
  public X509Encoding = X509Encoding;
  public TransformMode = TransformMode;
  public Number = Number;

  accepts = MediaType.Default;

  @ViewChild('stepper') stepper?: MatStepper;
  isLinear = false;
  error?: ProblemDetails;

  params = new FragmentParams();

  completedGetStarted = true;
  get completedEntries() {
    return this.config.input != null && this.config.input.length > 0;
  }
  get completedMode() {
    return this.config.mode != null && this.config.mode != TransformMode.None;
  }

  completedDone = false;

  constructor(private router: Router, private route: ActivatedRoute, public dialog: MatDialog, private snackBar: MatSnackBar) {
  }

  ngOnInit(): void {
    this.params = FragmentParams.parse(this.route);
  }

  selectedIndexChange(value:any) {
    //this.params.stepperIndex = value.selectedIndex;
    //this.params.updateUrl(this.router);
  }

  config = {} as ConfigTransform;

  get entries () : X509File[] | undefined {
    return this.config.input;
  }

  set entries(value) {
    if(value == null) {
      delete this.config.input;
    } else {
      this.config.input = value;
    }
  }

  create() {
    this.error = undefined;
    const dialogRef = this.dialog.open(ExecuteDialogComponent, {
      data: { accepts: this.accepts, transform: this.config ?? {} } as ExecuteDialogData,
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
    this.config = {} as ConfigTransform;
    this.stepper?.reset();
    timer(0).subscribe({
      next:() => {
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

  get ready() {
    return this.completedMode && this.completedEntries;
  }
}
