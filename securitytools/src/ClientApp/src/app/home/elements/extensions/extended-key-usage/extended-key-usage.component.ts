import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ConfigExtendedKeyUsageExtension, ExtendedKeyUsageFlags } from 'src/app/services/models';

@Component({
  selector: 'app-extended-key-usage',
  templateUrl: './extended-key-usage.component.html',
  styleUrls: ['./extended-key-usage.component.css']
})
export class ExtendedKeyUsageComponent {
  critical = false;
  serverAuth = false;
  clientAuth = false;
  codeSigning = false;
  emailProtection = false;
  timeStamping = false;
  ocspSigning = false;
  oids = [] as string[];

  @Input() set config(config: ConfigExtendedKeyUsageExtension | undefined) {
    if(config == null) {
      this.critical = false;
      this.serverAuth = false;
      this.clientAuth = false;
      this.codeSigning = false;
      this.emailProtection = false;
      this.timeStamping = false;
      this.ocspSigning = false;
      this.oids = [] as string[];
      return;
    }
    this.critical = config.critical ?? false;
    this.serverAuth = ((config.extendedKeyUsages & ExtendedKeyUsageFlags.ServerAuth) == ExtendedKeyUsageFlags.ServerAuth);
    this.clientAuth = ((config.extendedKeyUsages & ExtendedKeyUsageFlags.ClientAuth) == ExtendedKeyUsageFlags.ClientAuth);
    this.codeSigning = ((config.extendedKeyUsages & ExtendedKeyUsageFlags.CodeSigning) == ExtendedKeyUsageFlags.CodeSigning);
    this.emailProtection = ((config.extendedKeyUsages & ExtendedKeyUsageFlags.EmailProtection) == ExtendedKeyUsageFlags.EmailProtection);
    this.timeStamping = ((config.extendedKeyUsages & ExtendedKeyUsageFlags.TimeStamping) == ExtendedKeyUsageFlags.TimeStamping);
    this.ocspSigning = ((config.extendedKeyUsages & ExtendedKeyUsageFlags.OCSPSigning) == ExtendedKeyUsageFlags.OCSPSigning);
    this.oids = config.oids ?? [] as string[];
  }

  @Output() configChange = new EventEmitter<ConfigExtendedKeyUsageExtension | undefined>();

  fire() {
    let config = { critical: this.critical, extendedKeyUsages: ExtendedKeyUsageFlags.None } as ConfigExtendedKeyUsageExtension;

    config.extendedKeyUsages += this.serverAuth ? ExtendedKeyUsageFlags.ServerAuth : ExtendedKeyUsageFlags.None;
    config.extendedKeyUsages += this.clientAuth ? ExtendedKeyUsageFlags.ClientAuth : ExtendedKeyUsageFlags.None;
    config.extendedKeyUsages += this.codeSigning ? ExtendedKeyUsageFlags.CodeSigning : ExtendedKeyUsageFlags.None;
    config.extendedKeyUsages += this.emailProtection ? ExtendedKeyUsageFlags.EmailProtection : ExtendedKeyUsageFlags.None;
    config.extendedKeyUsages += this.timeStamping ? ExtendedKeyUsageFlags.TimeStamping : ExtendedKeyUsageFlags.None;
    config.extendedKeyUsages += this.ocspSigning ? ExtendedKeyUsageFlags.OCSPSigning : ExtendedKeyUsageFlags.None;

    config.oids = this.oids;
    if(this.oids.length == 0) {
      delete config.oids;
    }

    if (config.extendedKeyUsages == ExtendedKeyUsageFlags.None && config.oids == null) {
      this.configChange.emit(undefined);
    } else {
      this.configChange.emit(config);
    }
  }
}
