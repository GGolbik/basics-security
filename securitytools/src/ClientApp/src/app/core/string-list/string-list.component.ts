import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-string-list',
  templateUrl: './string-list.component.html',
  styleUrls: ['./string-list.component.css']
})
export class StringListComponent {

  list = [] as string[];

  @Input() title = "";
  @Input() label = "";
  @Input() placeholder = "";
  
  @Input() type = "text";

  @Input() set config(list: string[]) {
    this.list = list;
  }

  @Output() configChange = new EventEmitter<string[]>();

  addEntry() {
    this.list.push("");
    this.fire();
  }

  deleteEntry(index: number) {
    this.list.splice(index, 1);
    this.fire();
  }

  setV(i:number, value:any) {
    this.list[i] = value.target.value;
    this.fire();
  }

  fire(){
    this.configChange.emit(this.list);
  }

}
