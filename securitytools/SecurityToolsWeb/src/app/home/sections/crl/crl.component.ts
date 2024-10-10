import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatStepper } from '@angular/material/stepper';
import { Router, ActivatedRoute } from '@angular/router';
import { timer } from 'rxjs';
import { ExecuteDialogComponent, ExecuteDialogData } from '../../execute-dialog/execute-dialog.component';
import { ConfigCrl, ConfigCrlValidity, ConfigKeyPair, CrlEntry, FragmentParams, MediaType, ProblemDetails, X509Encoding, X509File } from '../../../services/models';
import { exportJson } from '../../../../main';

@Component({
  selector: 'app-crl',
  templateUrl: './crl.component.html',
  styleUrls: ['./crl.component.css']
})
export class CrlComponent  implements OnInit {
  public MediaType = MediaType;
  public X509Encoding = X509Encoding;
  public Number = Number;

  accepts = MediaType.Default;
  fileFormat? = X509Encoding.Default;

  @ViewChild('stepper') stepper?: MatStepper;
  isLinear = false;
  error?: ProblemDetails;

  params = new FragmentParams();

  completedGetStarted = true;
  get completedIssuer() {
    return this.config.issuer?.data != null;
  }
  get completedCrlNumber() {
    return this.config.crlNumber != null && this.config.crlNumber.length > 0;
  }
  get completedValidity() {
    return this.config.validity?.thisUpdate != null || this.config.validity?.nextUpdate != null;
  }
  get completedHashAlgorithm() {
    return this.config.hashAlgorithm != null;
  }
  get completedEntries() {
    return this.config.crlEntries != null && this.config.crlEntries.length > 0;
  }
  get completedCrl() {
    return this.config.crl?.data != null;
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

  config = {} as ConfigCrl;

  get issuer () : X509File | undefined {
    return this.config.issuer;
  }

  set issuer(value) {
    if(value == null) {
      delete this.config.issuer;
    } else {
      this.config.issuer = value;
    }
  }

  get keyPair () : ConfigKeyPair | undefined {
    return this.config.keyPair;
  }

  set keyPair(value) {
    if(value == null) {
      delete this.config.keyPair;
    } else {
      this.config.keyPair = value;
    }
  }

  
  get crl () : string | undefined {
    return this.config.crl?.data;
  }

  set crl(value) {
    if(value == null) {
      delete this.config.crl;
    } else {
      this.config.crl ??= {} as X509File;
      this.config.crl.data = value;
    }
  }

  get crlEntries () : CrlEntry[] | undefined {
    return this.config.crlEntries;
  }

  set crlEntries(value) {
    if(value == null) {
      delete this.config.crlEntries;
    } else {
      this.config.crlEntries = value;
    }
  }
  
  get crlNumber () : string | undefined {
    return this.config.crlNumber;
  }

  set crlNumber(value) {
    if(value == null) {
      delete this.config.crlNumber;
    } else {
      this.config.crlNumber = value;
    }
  }

  get validity () : ConfigCrlValidity | undefined {
    return this.config.validity;
  }

  set validity(value) {
    if(value == null) {
      delete this.config.validity;
    } else {
      this.config.validity = value;
    }
  }
  create() {
    this.error = undefined;
    let config = this.config ?? {};
    if (this.fileFormat != null) {
      config.crl ??= {};
      config.crl.fileFormat ??= {};
      config.crl.fileFormat.encoding = this.fileFormat;
      config.issuer ??= {};
      config.issuer.fileFormat ??= {};
      config.issuer.fileFormat.encoding = this.fileFormat;
      config.keyPair ??= {};
      config.keyPair.privateKey ??= {};
      config.keyPair.privateKey.fileFormat ??= {};
      if ((config.keyPair.privateKey.fileFormat.encoding ?? X509Encoding.Default) == X509Encoding.Default) {
        config.keyPair.privateKey.fileFormat.encoding = this.fileFormat;
      }
      config.keyPair.publicKey ??= {};
      config.keyPair.publicKey.fileFormat ??= {};
      config.keyPair.publicKey.fileFormat.encoding ??= this.fileFormat;
    }
    const dialogRef = this.dialog.open(ExecuteDialogComponent, {
      data: { accepts: this.accepts, crl: config } as ExecuteDialogData,
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
    this.config = {} as ConfigCrl;
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
    return this.completedIssuer;
  }
}
