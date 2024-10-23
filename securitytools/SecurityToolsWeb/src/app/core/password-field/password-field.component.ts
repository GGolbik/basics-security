import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-password-field',
  templateUrl: './password-field.component.html',
  styleUrl: './password-field.component.css'
})
export class PasswordFieldComponent {
  @Input() label = "Password";
  @Input() placeholder = "Password";
  @Input() hint = "";
  @Input() hidePassword = true;
  @Input() password: string | undefined | null;
  @Output() passwordChange = new EventEmitter<string | undefined | null>();

  change() {
    this.passwordChange.emit(this.password);
  }
}
