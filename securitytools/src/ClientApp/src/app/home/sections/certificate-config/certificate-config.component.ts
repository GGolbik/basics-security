import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatStepper } from '@angular/material/stepper';
import { Router, ActivatedRoute } from '@angular/router';
import { timer } from 'rxjs';
import { ExecuteDialogComponent, ExecuteDialogData } from 'src/app/home/execute-dialog/execute-dialog.component';
import { MediaType, X509Encoding, ExtensionsDefaults, ProblemDetails, FragmentParams, ConfigCsr, ConfigKeyPair, ConfigSubjectName, ConfigBasicConstraintsExtension, ConfigExtensions, ConfigAuthorityKeyIdentifierExtension, ConfigSubjectKeyIdentifierExtension, ConfigSubjectAlternativeName, ConfigKeyUsageExtension, ConfigExtendedKeyUsageExtension, ConfigExtensionDefault, ConfigCert, X509File, CertificateRequestLoadOptions } from 'src/app/services/models';
import { base64ToByteLength, exportJson } from 'src/main';

@Component({
  selector: 'app-certificate-config',
  templateUrl: './certificate-config.component.html',
  styleUrls: ['./certificate-config.component.css']
})
export class CertificateConfigComponent implements OnInit {
  public MediaType = MediaType;
  public X509Encoding = X509Encoding;
  public Number = Number;
  public ExtensionsDefaults = ExtensionsDefaults;
  public CertificateRequestLoadOptions = CertificateRequestLoadOptions;

  accepts = MediaType.Default;
  fileFormat? = X509Encoding.Default;

  @ViewChild('stepper') stepper?: MatStepper;
  isLinear = false;
  error?: ProblemDetails;

  params = new FragmentParams();

  private _selfSigned = false;
  get selfSigned() {
    return this._selfSigned;
  }
  set selfSigned(value) {
    this._selfSigned = value;
    this.keyPair = undefined;
    if(value) {
      this.issuer = undefined;
    }
  }

  completedGetStarted = true;
  get completedIssuer() {
    return this.issuer != null || this.selfSigned;
  }
  get completedKeyPair() {
    return this.keyPair != null;
  }
  get completedCsr() {
    return this.csr != null;
  }
  get completedSerialNumber() {
    return this.serialNumber != null;
  }
  get completedValidity() {
    return this.validity != null;
  }
  get completedHashAlgorithm() {
    return this.hashAlgorithm != null;
  }
  get completedBasicConstraints() {
    return this.basicConstraints != null;
  }
  get completedAuthorityKeyIdentifier() {
    return this.authorityKeyIdentifier != null;
  }
  get completedSubjectKeyIdentifier() {
    return this.subjectKeyIdentifier != null;
  }
  get completedSubjectAlternativeName() {
    return this.subjectAlternativeName != null;
  }
  get completedKeyUsage() {
    return this.keyUsage != null;
  }
  get completedExtendedKeyUsage() {
    return this.extendedKeyUsage != null;
  }
  get compeltedExtensions() {
    return this.extensions != null;
  }
  completedDone = false;

  constructor(private router: Router, private route: ActivatedRoute, public dialog: MatDialog, private snackBar: MatSnackBar) {
  }

  ngOnInit(): void {
    this.params = FragmentParams.parse(this.route);
  }

  selectedIndexChange(value: any) {
    //this.params.stepperIndex = value.selectedIndex;
    //this.params.updateUrl(this.router);
  }

  config = {} as ConfigCert;

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

  get issuer () : X509File | undefined {
    return this.config.issuer;
  }

  set issuer(value) {
    if(value == null || ((value.data == null || value.data.length == 0) && value.fileName == null)) {
      delete this.config.issuer;
    } else {
      this.config.issuer = value;
    }
  }

  get csrLoadOptions() {
    return this.config.csrLoadOptions ?? CertificateRequestLoadOptions.Default;
  }
  set csrLoadOptions(value) {
    if(value == null || value == CertificateRequestLoadOptions.Default) {
      delete this.config.csrLoadOptions;
    } else {
      this.config.csrLoadOptions = value;
    }
  }
  
  get hashAlgorithm() {
    return this.config.hashAlgorithm;
  }
  set hashAlgorithm(value) {
    if(value == null) {
      delete this.config.hashAlgorithm;
    } else {
      this.config.hashAlgorithm = value;
    }
  }

  get csrLoadOptionsSkipSignatureValidation() {
    if(this.csrLoadOptions == null) {
      return false;
    }
    return (this.config.csrLoadOptions! & CertificateRequestLoadOptions.SkipSignatureValidation) == CertificateRequestLoadOptions.SkipSignatureValidation;
  }

  set csrLoadOptionsSkipSignatureValidation(value) {
    if(value) {
      this.csrLoadOptions += CertificateRequestLoadOptions.SkipSignatureValidation;
    } else {
      this.csrLoadOptions -= CertificateRequestLoadOptions.SkipSignatureValidation;
    }
  }

  get csrLoadOptionsUnsafeLoadCertificateExtensions() {
    if(this.csrLoadOptions == null) {
      return false;
    }
    return (this.config.csrLoadOptions! & CertificateRequestLoadOptions.UnsafeLoadCertificateExtensions) == CertificateRequestLoadOptions.UnsafeLoadCertificateExtensions;
  }
  
  set csrLoadOptionsUnsafeLoadCertificateExtensions(value) {
    if(value) {
      this.csrLoadOptions += CertificateRequestLoadOptions.UnsafeLoadCertificateExtensions;
    } else {
      this.csrLoadOptions -= CertificateRequestLoadOptions.UnsafeLoadCertificateExtensions;
    }
  }

  get csr() {
    return this.config.csr;
  }
  set csr(value) {
    if(value == null) {
      delete this.config.csr;
    } else {
      this.config.csr = value;
    }
  }

  get csrFile() {
    return this.csr?.csr?.data;
  }
  set csrFile(value) {
    let csr = this.csr;
    csr ??= {} as ConfigCsr;
    csr.csr ??= {} as X509File;
    csr.csr.data = value;
    this.csr = csr;
  }

  get serialNumber() {
    return this.config.serialNumber;
  }
  set serialNumber(value) {
    if(value == null) {
      delete this.config.serialNumber;
    } else {
      this.config.serialNumber = value;
    }
  }

  get validity() {
    return this.config.validity;
  }

  set validity(value) {
    this.config.validity = value;
  }

  get basicConstraints() : ConfigBasicConstraintsExtension | undefined {
    return this.config.extensions?.basicConstraints;
  }

  set basicConstraints(config: ConfigBasicConstraintsExtension | undefined) {
    if(this.config.extensions == null) {
      this.config.extensions = {} as ConfigExtensions;
    }
    this.config.extensions.basicConstraints = config;
    if(this.config.extensions.basicConstraints == null) {
      delete this.config.extensions.basicConstraints;
    }
    if(Object.keys(this.config.extensions).length == 0) {
      delete this.config.extensions;
    }
  }

  get authorityKeyIdentifier() : ConfigAuthorityKeyIdentifierExtension | undefined {
    return this.config.extensions?.authorityKeyIdentifier;
  }

  set authorityKeyIdentifier(config: ConfigAuthorityKeyIdentifierExtension | undefined) {
    if(this.config.extensions == null) {
      this.config.extensions = {} as ConfigExtensions;
    }
    this.config.extensions.authorityKeyIdentifier = config;
    if(this.config.extensions.authorityKeyIdentifier == null) {
      delete this.config.extensions.authorityKeyIdentifier;
    }
    if(Object.keys(this.config.extensions).length == 0) {
      delete this.config.extensions;
    }
  }
  
  get subjectKeyIdentifier() : ConfigSubjectKeyIdentifierExtension | undefined {
    return this.config.extensions?.subjectKeyIdentifier;
  }

  set subjectKeyIdentifier(config: ConfigSubjectKeyIdentifierExtension | undefined) {
    if(this.config.extensions == null) {
      this.config.extensions = {} as ConfigExtensions;
    }
    this.config.extensions.subjectKeyIdentifier = config;
    if(this.config.extensions.subjectKeyIdentifier == null) {
      delete this.config.extensions.subjectKeyIdentifier;
    }
    if(Object.keys(this.config.extensions).length == 0) {
      delete this.config.extensions;
    }
  }

  get subjectAlternativeName() : ConfigSubjectAlternativeName | undefined {
    return this.config.extensions?.subjectAlternativeName;
  }

  set subjectAlternativeName(config: ConfigSubjectAlternativeName | undefined) {
    if(this.config.extensions == null) {
      this.config.extensions = {} as ConfigExtensions;
    }
    this.config.extensions.subjectAlternativeName = config;
    if(this.config.extensions.subjectAlternativeName == null) {
      delete this.config.extensions.subjectAlternativeName;
    }
    if(Object.keys(this.config.extensions).length == 0) {
      delete this.config.extensions;
    }
  }

  get keyUsage() : ConfigKeyUsageExtension | undefined {
    return this.config.extensions?.keyUsage;
  }

  set keyUsage(config: ConfigKeyUsageExtension | undefined) {
    if(this.config.extensions == null) {
      this.config.extensions = {} as ConfigExtensions;
    }
    this.config.extensions.keyUsage = config;
    if(this.config.extensions.keyUsage == null) {
      delete this.config.extensions.keyUsage;
    }
    if(Object.keys(this.config.extensions).length == 0) {
      delete this.config.extensions;
    }
  }

  get extendedKeyUsage() : ConfigExtendedKeyUsageExtension | undefined {
    return this.config.extensions?.extendedKeyUsage;
  }

  set extendedKeyUsage(config: ConfigExtendedKeyUsageExtension | undefined) {
    if(this.config.extensions == null) {
      this.config.extensions = {} as ConfigExtensions;
    }
    this.config.extensions.extendedKeyUsage = config;
    if(this.config.extensions.extendedKeyUsage == null) {
      delete this.config.extensions.extendedKeyUsage;
    }
    if(Object.keys(this.config.extensions).length == 0) {
      delete this.config.extensions;
    }
  }

  get extensions() : ConfigExtensionDefault[] | undefined {
    return this.config.extensions?.extensions;
  }

  set extensions(config: ConfigExtensionDefault[] | undefined) {
    if(this.config.extensions == null) {
      this.config.extensions = {} as ConfigExtensions;
    }
    this.config.extensions.extensions = config;
    if(this.config.extensions.extensions == null) {
      delete this.config.extensions.extensions;
    }
    if(Object.keys(this.config.extensions).length == 0) {
      delete this.config.extensions;
    }
  }

  create() {
    this.error = undefined;
    const dialogRef = this.dialog.open(ExecuteDialogComponent, {
      data: { accepts: this.accepts, cert: this.config } as ExecuteDialogData,
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

  get warnings() {
    let warnings: string[] = [];
    if(base64ToByteLength(this.serialNumber) > 20) {
      warnings.push("Conformant CAs MUST NOT use serial numbers longer than 20 octets.")
    }
    return warnings;
  }

  reset() {
    this.completedGetStarted = false;
    this.completedDone = false;
    this.config = {} as ConfigCert;
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
  get ready() {
    return this.completedCsr && this.completedIssuer;
  }
}
