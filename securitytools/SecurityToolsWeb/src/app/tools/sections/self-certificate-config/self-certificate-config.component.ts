import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MatStepper } from '@angular/material/stepper';
import { MatDialog } from '@angular/material/dialog';
import { exportJson } from '../../../../main';
import { MatSnackBar } from '@angular/material/snack-bar';
import { timer } from 'rxjs';
import { ExecuteDialogComponent, ExecuteDialogData } from '../../execute-dialog/execute-dialog.component';
import { ExtensionsDefaults, CertificateRequestLoadOptions, FragmentParams, ConfigCert, ConfigKeyPair, MediaType, X509Encoding, ConfigAuthorityKeyIdentifierExtension, ConfigBasicConstraintsExtension, ConfigExtendedKeyUsageExtension, ConfigExtensionDefault, ConfigExtensions, ConfigKeyUsageExtension, ConfigSubjectAlternativeName, ConfigSubjectKeyIdentifierExtension, ConfigSubjectName, ProblemDetails, ConfigCsr } from '../../services/models';

@Component({
  selector: 'app-self-certificate-config',
  templateUrl: './self-certificate-config.component.html',
  styleUrls: ['./self-certificate-config.component.css']
})
export class SelfCertificateConfigComponent implements OnInit {
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

  selfSigned = false;

  completedGetStarted = true;
  get completedKeyPair() {
    return this.keyPair != null;
  }
  get completedValidity() {
    return this.validity != null;
  }
  get completedSubjectName() {
    return this.subjectName != null;
  }

  get completedSerialNumber() {
    return this.serialNumber != null;
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

  get configCsr() {
    this.config.csr ??= {} as ConfigCsr;
    return this.config.csr;
  }

  get keyPair(): ConfigKeyPair | undefined {
    return this.configCsr.keyPair;
  }

  set keyPair(value) {
    if (value == null) {
      delete this.configCsr.keyPair;
    } else {
      this.configCsr.keyPair = value;
    }
  }

  get hashAlgorithm() {
    return this.config.hashAlgorithm;
  }
  set hashAlgorithm(value) {
    if (value == null) {
      delete this.config.hashAlgorithm;
    } else {
      this.config.hashAlgorithm = value;
    }
  }

  get serialNumber() {
    return this.config.serialNumber;
  }
  set serialNumber(value) {
    if (value == null) {
      delete this.config.serialNumber;
    } else {
      this.config.serialNumber = value;
    }
  }

  get subjectName(): ConfigSubjectName | undefined {
    return this.configCsr.subjectName;
  }

  set subjectName(value) {
    if (value == null) {
      delete this.configCsr.subjectName;
    } else {
      this.configCsr.subjectName = value;
      if (Object.keys(this.configCsr.subjectName).length == 0) {
        delete this.configCsr.subjectName;
      }
    }
  }

  get validity() {
    return this.config.validity;
  }

  set validity(value) {
    this.config.validity = value;
  }

  get basicConstraints(): ConfigBasicConstraintsExtension | undefined {
    return this.configCsr.extensions?.basicConstraints;
  }

  set basicConstraints(config: ConfigBasicConstraintsExtension | undefined) {
    if (this.configCsr.extensions == null) {
      this.configCsr.extensions = {} as ConfigExtensions;
    }
    this.configCsr.extensions.basicConstraints = config;
    if (this.configCsr.extensions.basicConstraints == null) {
      delete this.configCsr.extensions.basicConstraints;
    }
    if (Object.keys(this.configCsr.extensions).length == 0) {
      delete this.configCsr.extensions;
    }
  }

  get authorityKeyIdentifier(): ConfigAuthorityKeyIdentifierExtension | undefined {
    return this.configCsr.extensions?.authorityKeyIdentifier;
  }

  set authorityKeyIdentifier(config: ConfigAuthorityKeyIdentifierExtension | undefined) {
    if (this.configCsr.extensions == null) {
      this.configCsr.extensions = {} as ConfigExtensions;
    }
    this.configCsr.extensions.authorityKeyIdentifier = config;
    if (this.configCsr.extensions.authorityKeyIdentifier == null) {
      delete this.configCsr.extensions.authorityKeyIdentifier;
    }
    if (Object.keys(this.configCsr.extensions).length == 0) {
      delete this.configCsr.extensions;
    }
  }

  get subjectKeyIdentifier(): ConfigSubjectKeyIdentifierExtension | undefined {
    return this.configCsr.extensions?.subjectKeyIdentifier;
  }

  set subjectKeyIdentifier(config: ConfigSubjectKeyIdentifierExtension | undefined) {
    if (this.configCsr.extensions == null) {
      this.configCsr.extensions = {} as ConfigExtensions;
    }
    this.configCsr.extensions.subjectKeyIdentifier = config;
    if (this.configCsr.extensions.subjectKeyIdentifier == null) {
      delete this.configCsr.extensions.subjectKeyIdentifier;
    }
    if (Object.keys(this.configCsr.extensions).length == 0) {
      delete this.configCsr.extensions;
    }
  }

  get subjectAlternativeName(): ConfigSubjectAlternativeName | undefined {
    return this.configCsr.extensions?.subjectAlternativeName;
  }

  set subjectAlternativeName(config: ConfigSubjectAlternativeName | undefined) {
    if (this.configCsr.extensions == null) {
      this.configCsr.extensions = {} as ConfigExtensions;
    }
    this.configCsr.extensions.subjectAlternativeName = config;
    if (this.configCsr.extensions.subjectAlternativeName == null) {
      delete this.configCsr.extensions.subjectAlternativeName;
    }
    if (Object.keys(this.configCsr.extensions).length == 0) {
      delete this.configCsr.extensions;
    }
  }

  get keyUsage(): ConfigKeyUsageExtension | undefined {
    return this.configCsr.extensions?.keyUsage;
  }

  set keyUsage(config: ConfigKeyUsageExtension | undefined) {
    if (this.configCsr.extensions == null) {
      this.configCsr.extensions = {} as ConfigExtensions;
    }
    this.configCsr.extensions.keyUsage = config;
    if (this.configCsr.extensions.keyUsage == null) {
      delete this.configCsr.extensions.keyUsage;
    }
    if (Object.keys(this.configCsr.extensions).length == 0) {
      delete this.configCsr.extensions;
    }
  }

  get extendedKeyUsage(): ConfigExtendedKeyUsageExtension | undefined {
    return this.configCsr.extensions?.extendedKeyUsage;
  }

  set extendedKeyUsage(config: ConfigExtendedKeyUsageExtension | undefined) {
    if (this.configCsr.extensions == null) {
      this.configCsr.extensions = {} as ConfigExtensions;
    }
    this.configCsr.extensions.extendedKeyUsage = config;
    if (this.configCsr.extensions.extendedKeyUsage == null) {
      delete this.configCsr.extensions.extendedKeyUsage;
    }
    if (Object.keys(this.configCsr.extensions).length == 0) {
      delete this.configCsr.extensions;
    }
  }

  get extensions(): ConfigExtensionDefault[] | undefined {
    return this.configCsr.extensions?.extensions;
  }

  set extensions(config: ConfigExtensionDefault[] | undefined) {
    if (this.configCsr.extensions == null) {
      this.configCsr.extensions = {} as ConfigExtensions;
    }
    this.configCsr.extensions.extensions = config;
    if (this.configCsr.extensions.extensions == null) {
      delete this.configCsr.extensions.extensions;
    }
    if (Object.keys(this.configCsr.extensions).length == 0) {
      delete this.config.extensions;
    }
  }

  create() {
    this.error = undefined;
    let config = this.config;
    if (this.fileFormat != null) {
      config.cert ??= {};
      config.cert.fileFormat ??= {};
      config.cert.fileFormat.encoding = this.fileFormat;
      config.csr ??= {};
      config.csr.csr ??= {};
      config.csr.csr.fileFormat ??= {};
      config.csr.csr.fileFormat.encoding = this.fileFormat;
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
      data: { accepts: this.accepts, cert: config } as ExecuteDialogData,
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

}
