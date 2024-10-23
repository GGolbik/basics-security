import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ConfigKeyPair, X509File } from '../../services/models';

@Component({
  selector: 'app-issuer',
  templateUrl: './issuer.component.html',
  styleUrls: ['./issuer.component.css']
})
export class IssuerComponent {
  public Number = Number;

  hidePassword = true;
  private _cert?: X509File;
  private _keyPair?: ConfigKeyPair;

  @Input() set cert(cert: X509File | undefined) {
    this._cert = cert;
  }

  @Output() certChange = new EventEmitter<X509File>();

  @Input() set keyPair(keyPair: ConfigKeyPair | undefined) {
    this._keyPair = keyPair;
  }

  @Output() keyPairChange = new EventEmitter<ConfigKeyPair | undefined>();

  get cert(): X509File {
    let cert = this._cert;
    cert ??= {} as X509File;
    return cert;
  }

  get keyPair(): ConfigKeyPair {
    let key = this._keyPair;
    key ??= {} as ConfigKeyPair;
    return key;
  }

  get key() : X509File | undefined {
    return this.keyPair.privateKey;
  }

  set key(value) {
    let keyPair = this.keyPair;
    keyPair.privateKey = value;
    this._keyPair = keyPair;
    this.fireKey();
  }
  

  fireCert() {
    this.certChange.emit(this._cert);
  }

  fireKey() {
    this.keyPairChange.emit(this._keyPair);
  }

  get password() {
    return this.cert?.password;
  }

  set password(value) {
    let cert = this.cert;
    cert ??= {} as X509File;
    cert.password = value;

    let key = this.key;
    key ??= {} as X509File;
    key.password = value;
    let keyPair = this.keyPair;
    keyPair.privateKey = key;

    this._cert = cert;
    this._keyPair = keyPair;
    this.fireKey();
    this.fireCert();
  }

  get certData() {
    return this.cert?.data ?? "";
  }

  set certData(value: string | undefined) {
    let config = this.cert;
    config ??= {} as X509File;
    config.data = value;

    this._cert = config;
    this.fireCert();
  }

  get keyData() {
    return this.key?.data ?? "";
  }

  set keyData(value: string | undefined) {
    let config = this.key;
    config ??= {} as X509File;
    config.data = value;

    this.key = config;
    this.fireKey();
  }

}
