import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ConfigBasicConstraintsExtension } from 'src/app/services/models';

@Component({
  selector: 'app-basic-constraints',
  templateUrl: './basic-constraints.component.html',
  styleUrls: ['./basic-constraints.component.css']
})
export class BasicConstraintsComponent {
  critical = false;
  ca = false;
  hasPathLengthConstraint = false;
  pathLengthConstraint = 0;

  @Input() set config(config: ConfigBasicConstraintsExtension | undefined) {
    if(config == null) {
      this.critical = false;
      this.ca = false;
      this.hasPathLengthConstraint = false;
      return;
    }
    this.critical = config.critical ?? false;
    this.ca = config.certificateAuthority ?? false;
    this.hasPathLengthConstraint = config.hasPathLengthConstraint ?? false;
    this.pathLengthConstraint = config.pathLengthConstraint ?? 0;
  }

  @Output() configChange = new EventEmitter<ConfigBasicConstraintsExtension | undefined>();

  fire() {
    let config = { critical: this.critical } as ConfigBasicConstraintsExtension;
    config.certificateAuthority = this.ca;
    config.hasPathLengthConstraint = this.hasPathLengthConstraint;
    config.pathLengthConstraint = config.hasPathLengthConstraint ? this.pathLengthConstraint : 0;
    this.configChange.emit(config);
  }
}
