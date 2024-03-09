import { Component, OnInit, ViewChild } from '@angular/core';
import { X509Encoding, MediaType, ProblemDetails, ConfigCsr, ConfigSubjectName, FragmentParams, ConfigAuthorityKeyIdentifierExtension, ConfigExtensions, ConfigSubjectKeyIdentifierExtension, ConfigBasicConstraintsExtension, ConfigExtensionDefault, ConfigExtendedKeyUsageExtension, ConfigKeyUsageExtension, ConfigSubjectAlternativeName, ConfigKeyPair, ExtensionsDefaults } from 'src/app/services/models';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { ExecuteDialogComponent, ExecuteDialogData } from 'src/app/home/execute-dialog/execute-dialog.component';
import { MatStepper } from '@angular/material/stepper';
import { timer } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';
import { exportJson } from 'src/main';

@Component({
  selector: 'app-csr-config',
  templateUrl: './csr-config.component.html',
  styleUrls: ['./csr-config.component.css']
})
export class CsrConfigComponent implements OnInit {
  public MediaType = MediaType;
  public X509Encoding = X509Encoding;
  public Number = Number;
  public ExtensionsDefaults = ExtensionsDefaults;

  accepts = MediaType.Default;
  fileFormat? = X509Encoding.Default;

  @ViewChild('stepper') stepper?: MatStepper;
  isLinear = false;
  error?: ProblemDetails;

  params = new FragmentParams();

  completedGetStarted = true;
  get completedKeyPair() {
    return this.keyPair != null;
  }
  get completedSubjectName() {
    return this.subjectName != null;
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

  config = {} as ConfigCsr;

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

  get subjectName () : ConfigSubjectName | undefined {
    return this.config.subjectName;
  }

  set subjectName(value) {
    if(value == null) {
      delete this.config.subjectName;
    } else {
      this.config.subjectName = value;
      if(Object.keys(this.config.subjectName).length == 0) {
        delete this.config.subjectName;
      }
    }
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
    let config = this.config;
    if (this.fileFormat != null) {
      config.csr ??= {};
      config.csr.fileFormat ??= {};
      config.csr.fileFormat.encoding = this.fileFormat;
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
      data: { accepts: this.accepts, csr: config } as ExecuteDialogData,
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
    if(this.config.subjectName == null) {
      warnings.push("Subject name is not specified.");
    }
    if(this.config.extensions?.basicConstraints == null) {
      warnings.push("Basic constraints extension is not specified. Should be included in CA certificates.");
    } else {
      // check if ca
      if(this.config.extensions.basicConstraints.certificateAuthority) {
        if(this.config.extensions?.authorityKeyIdentifier == null) {
          warnings.push("Authority Key Identifier extension is not specified. Must appear in all CA certificates which contain the basic constraints extension. This is only optional for self-signed CA certificates.");
        }
      }
    }
    if(this.config.extensions?.authorityKeyIdentifier?.critical) {
      warnings.push("Authority Key Identifier extension must be non-critical.")
    }
    if(this.config.extensions?.subjectKeyIdentifier == null) {
      warnings.push("Subject Key Identifier extension is not specified. Must appear in all CA certificates which contain the basic constraints extension. Should be included in all end entity certificates.");
    }
    if(this.config.subjectName == null && this.config.extensions?.subjectAlternativeName?.critical != true) {
      warnings.push("If the subject field contains an empty sequence, the subjectAltName extension MUST be marked critical.")
    }
    return warnings;
  }

  reset() {
    this.completedGetStarted = false;
    this.completedDone = false;
    this.config = {} as ConfigCsr;
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
