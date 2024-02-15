import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ConfigKeyUsageExtension, X509KeyUsageFlags } from 'src/app/services/models';

@Component({
  selector: 'app-key-usage',
  templateUrl: './key-usage.component.html',
  styleUrls: ['./key-usage.component.css']
})
export class KeyUsageComponent {
  critical = false;
  _encipherOnly = false;
  crlSign = false;
  keyCertSign = false;
  keyAgreement = false;
  dataEncipherment = false;
  keyEncipherment = false;
  nonRepudiation = false;
  digitalSignature = false;
  _decipherOnly = false;

  @Input() set config(config: ConfigKeyUsageExtension | undefined) {
    if(config == null) {
      this.critical = false;
      this._encipherOnly = false;
      this.crlSign = false;
      this.keyCertSign = false;
      this.keyAgreement = false;
      this.dataEncipherment = false;
      this.keyEncipherment = false;
      this.nonRepudiation = false;
      this.digitalSignature = false;
      this._decipherOnly = false;
      return;
    }
    this.critical = config.critical ?? false;
    this._encipherOnly = ((config.keyUsages & X509KeyUsageFlags.EncipherOnly) == X509KeyUsageFlags.EncipherOnly);
    this.crlSign = ((config.keyUsages & X509KeyUsageFlags.CrlSign) == X509KeyUsageFlags.CrlSign);
    this.keyCertSign = ((config.keyUsages & X509KeyUsageFlags.KeyCertSign) == X509KeyUsageFlags.KeyCertSign);
    this.keyAgreement = ((config.keyUsages & X509KeyUsageFlags.KeyAgreement) == X509KeyUsageFlags.KeyAgreement);
    this.dataEncipherment = ((config.keyUsages & X509KeyUsageFlags.DataEncipherment) == X509KeyUsageFlags.DataEncipherment);
    this.keyEncipherment = ((config.keyUsages & X509KeyUsageFlags.KeyEncipherment) == X509KeyUsageFlags.KeyEncipherment);
    this.nonRepudiation = ((config.keyUsages & X509KeyUsageFlags.NonRepudiation) == X509KeyUsageFlags.NonRepudiation);
    this.digitalSignature = ((config.keyUsages & X509KeyUsageFlags.DigitalSignature) == X509KeyUsageFlags.DigitalSignature);
    this._decipherOnly = ((config.keyUsages & X509KeyUsageFlags.DecipherOnly) == X509KeyUsageFlags.DecipherOnly);
  }

  @Output() configChange = new EventEmitter<ConfigKeyUsageExtension | undefined>();

  get decipherOnly() {
    return this._decipherOnly;
  }

  set decipherOnly(v) {
    this._decipherOnly = v;
    if(v) {
      this._encipherOnly = false;
    }
  }

  get encipherOnly() {
    return this._encipherOnly;
  }

  set encipherOnly(v) {
    this._encipherOnly = v;
    if(v) {
      this._decipherOnly = false;
    }
  }

  fire() {
    let config = { critical: this.critical, keyUsages: X509KeyUsageFlags.None };

    config.keyUsages += this.keyAgreement && this._encipherOnly ? X509KeyUsageFlags.EncipherOnly : X509KeyUsageFlags.None;
    config.keyUsages += this.crlSign ? X509KeyUsageFlags.CrlSign : X509KeyUsageFlags.None;
    config.keyUsages += this.keyCertSign ? X509KeyUsageFlags.KeyCertSign : X509KeyUsageFlags.None;
    config.keyUsages += this.keyAgreement ? X509KeyUsageFlags.KeyAgreement : X509KeyUsageFlags.None;
    config.keyUsages += this.dataEncipherment ? X509KeyUsageFlags.DataEncipherment : X509KeyUsageFlags.None;
    config.keyUsages += this.keyEncipherment ? X509KeyUsageFlags.KeyEncipherment : X509KeyUsageFlags.None;
    config.keyUsages += this.nonRepudiation ? X509KeyUsageFlags.NonRepudiation : X509KeyUsageFlags.None;
    config.keyUsages += this.digitalSignature ? X509KeyUsageFlags.DigitalSignature : X509KeyUsageFlags.None;
    config.keyUsages += this.keyAgreement && this._decipherOnly ? X509KeyUsageFlags.DecipherOnly : X509KeyUsageFlags.None;

    if (config.keyUsages == X509KeyUsageFlags.None) {
      this.configChange.emit(undefined);
    } else {
      this.configChange.emit(config);
    }
  }
}
