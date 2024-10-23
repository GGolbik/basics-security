import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CrlEntry, X509File } from '../../services/models';

@Component({
  selector: 'app-crl-entries',
  templateUrl: './crl-entries.component.html',
  styleUrls: ['./crl-entries.component.css']
})
export class CrlEntriesComponent {

  _entries: CrlEntry[] = [];

  @Input() set config(list: CrlEntry[] | undefined) {
    this._entries = list ?? [];
  }

  @Output() configChange = new EventEmitter<CrlEntry[] | undefined>();

  addEntry() {
    this._entries.push({ cert: {} as X509File } as CrlEntry);
    this.fire();
  }

  deleteEntry(index: number) {
    this._entries.splice(index, 1);
    this.fire();
  }

  setV(i:number, value:any) {
    this._entries[i] = value.target.value;
    this.fire();
  }

  fire(){
    this.configChange.emit(this._entries.length == 0 ? undefined : this._entries);
  }
}
