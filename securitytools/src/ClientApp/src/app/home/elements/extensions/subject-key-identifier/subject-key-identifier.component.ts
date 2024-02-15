import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ConfigSubjectKeyIdentifierExtension } from 'src/app/services/models';

@Component({
  selector: 'app-subject-key-identifier',
  templateUrl: './subject-key-identifier.component.html',
  styleUrls: ['./subject-key-identifier.component.css']
})
export class SubjectKeyIdentifierComponent {
  critical = false;
  ca = false;
  hasPathLengthConstraint = false;
  pathLengthConstraint = 0;

  @Input() set config(config: ConfigSubjectKeyIdentifierExtension | undefined) {
    if(config == null) {
      this.critical = false;
      return;
    }
    this.critical = config.critical ?? false;
  }

  @Output() configChange = new EventEmitter<ConfigSubjectKeyIdentifierExtension | undefined>();

  fire() {
    let config = { critical: this.critical } as ConfigSubjectKeyIdentifierExtension;
    this.configChange.emit(config);
  }
}
