import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ConfigAuthorityKeyIdentifierExtension } from 'src/app/services/models';

@Component({
  selector: 'app-authority-key-identifier',
  templateUrl: './authority-key-identifier.component.html',
  styleUrls: ['./authority-key-identifier.component.css']
})
export class AuthorityKeyIdentifierComponent {
  critical = false;
  includeKeyIdentifier = false;
  includeIssuerAndSerial = false;

  @Input() set config(config: ConfigAuthorityKeyIdentifierExtension | undefined) {
    if(config == null) {
      this.critical = false;
      this.includeKeyIdentifier = false;
      this.includeIssuerAndSerial = false;
      return;
    }
    this.critical = config.critical ?? false;
    this.includeKeyIdentifier = config.includeKeyIdentifier ?? false;
    this.includeIssuerAndSerial = config.includeIssuerAndSerial ?? false;
  }

  @Output() configChange = new EventEmitter<ConfigAuthorityKeyIdentifierExtension | undefined>();

  fire() {
    let config = { critical: this.critical } as ConfigAuthorityKeyIdentifierExtension;
    config.includeKeyIdentifier = this.includeKeyIdentifier;
    config.includeIssuerAndSerial = this.includeIssuerAndSerial;
    this.configChange.emit(config);
  }
}
