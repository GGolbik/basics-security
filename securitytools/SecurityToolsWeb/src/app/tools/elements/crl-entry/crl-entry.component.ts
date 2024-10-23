import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CrlEntry, X509File } from '../../services/models';

@Component({
  selector: 'app-crl-entry',
  templateUrl: './crl-entry.component.html',
  styleUrls: ['./crl-entry.component.css']
})
export class CrlEntryComponent {

  _entry = { crlEntry: { cert: {} as X509File } as CrlEntry } as CrlItem;

  @Input() set entry(crlEntry: CrlEntry | undefined) {
    this._entry.crlEntry = crlEntry ?? {} as CrlEntry;
    this._entry.crlEntry.cert ??= {} as X509File;
  }

  @Output() entryChange = new EventEmitter<CrlEntry | undefined>();

  fire() {
    this.entryChange.emit(this._entry.crlEntry);
  }
  @Output() onDeleteChange = new EventEmitter<CrlEntry | undefined>();

  protected deleteEntry() {
    this.onDeleteChange.emit(this._entry.crlEntry);
  }
}

interface CrlItem {
  name?: string;
  crlEntry: CrlEntry;
}
