import { Component, EventEmitter, Input, Output } from '@angular/core';
import { X509File } from '../../../services/models';

@Component({
  selector: 'app-file-entries',
  templateUrl: './file-entries.component.html',
  styleUrls: ['./file-entries.component.css']
})
export class FileEntriesComponent {

  _entries: X509File[] = [];

  @Input() set config(list: X509File[] | undefined) {
    this._entries = list ?? [];
  }

  @Output() configChange = new EventEmitter<X509File[] | undefined>();

  addEntry() {
    this._entries.push({} as X509File);
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
