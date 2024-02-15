import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ConfigKeyPair, SignatureAlgorithmName, X509Encoding, X509File, X509FileFormat } from 'src/app/services/models';

@Component({
  selector: 'app-privatekey',
  templateUrl: './privatekey.component.html',
  styleUrls: ['./privatekey.component.css']
})
export class PrivatekeyComponent {
  public X509Encoding = X509Encoding;
  public SignatureAlgorithmName = SignatureAlgorithmName;
  public Number = Number;

  readonly optionsRsa: number[] = [2048, 4096];
  readonly optionsEcdsa: string[] = [
    "brainpoolP160r1",
    "brainpoolP160t1",
    "brainpoolP192r1",
    "brainpoolP192t1",
    "brainpoolP224r1",
    "brainpoolP224t1",
    "brainpoolP256r1",
    "brainpoolP256t1",
    "brainpoolP320r1",
    "brainpoolP320t1",
    "brainpoolP384r1",
    "brainpoolP384t1",
    "brainpoolP512r1",
    "brainpoolP512t1",
    "nistP256",
    "nistP384",
    "nistP521"
  ];

  hidePassword = true;
  @Input() showHashAlgorithm = true;
  private _generate?: boolean;
  private _config?: ConfigKeyPair;

  @Input() set config(config: ConfigKeyPair | undefined) {
    this._config = config;
    if (config == null) {
      this._generate = true;
    } else if (config?.privateKey?.data != null) {
      this._generate = false;
    }
  }

  @Output() configChange = new EventEmitter<ConfigKeyPair>();

  get config(): ConfigKeyPair {
    let config = this._config;
    config ??= {} as ConfigKeyPair;
    config.privateKey ??= {} as X509File;
    return config;
  }

  fire() {
    this.configChange.emit(this._config);
  }

  get generate() {
    return this._generate ?? true;
  }

  set generate(value) {
    this.algorithm = SignatureAlgorithmName.Default;
    if(value != true) {
      this.hashAlgorithm = undefined;
    }
    this._generate = value;

    this.fire();
  }

  get algorithm() {
    return this.config.signatureAlgorithm ?? SignatureAlgorithmName.Default;
  }

  set algorithm(value) {
    let config = this.config;
    config.signatureAlgorithm = value == SignatureAlgorithmName.Default ? undefined : value;
    if (config.signatureAlgorithm == null) {
      config.eccurve = undefined;
      config.keySize = undefined;
    }
    this._config = config;
    this.fire();
  }

  set algorithmChange(event: any) {
    let config = this.config;
    config.signatureAlgorithm = event.value;
    this._config = config;
    this.fire();
  }

  get keySize() {
    return this.config?.keySize;
  }

  set keySize(value) {
    let config = this.config;
    config.keySize = value;

    this._config = config;
    this.fire();
  }

  get eccurve() {
    return this.config?.eccurve;
  }

  set eccurve(value) {
    let config = this.config;
    config.eccurve = value;

    this._config = config;
    this.fire();
  }

  get password() {
    return this.config?.privateKey?.password;
  }

  set password(value) {
    let config = this.config;
    config.privateKey ??= {} as X509File;
    config.privateKey.password = value;

    this._config = config;
    this.fire();
  }

  get data() {
    return this.config?.privateKey?.data ?? "";
  }

  set data(value: string | undefined) {
    let config = this.config;
    config.privateKey ??= {} as X509File;
    config.privateKey.data = value;

    this._config = config;
    this.fire();
  }

  get encoding() {
    return this.config?.privateKey?.fileFormat?.encoding ?? X509Encoding.Default;
  }

  set encoding(value) {
    let config = this.config;
    config.privateKey ??= {} as X509File;
    config.privateKey.fileFormat ??= {} as X509FileFormat;
    config.privateKey.fileFormat.encoding = value == X509Encoding.Default ? undefined : value;

    this._config = config;
    this.fire();
  }

  get hashAlgorithm() {
    return this.config?.privateKey?.hashAlgorithm;
  }

  set hashAlgorithm(value) {
    let config = this.config;
    config.privateKey ??= {} as X509File;
    config.privateKey.hashAlgorithm = value;

    this._config = config;
    this.fire();
  }

}
