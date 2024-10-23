import { Component, EventEmitter, Input, Output } from '@angular/core';
import { HashAlgorithmNameOrOid, HashAlgorithmNames } from '../../services/models';

@Component({
  selector: 'app-hash-algorithm',
  templateUrl: './hash-algorithm.component.html',
  styleUrls: ['./hash-algorithm.component.css']
})
export class HashAlgorithmComponent {
  _hashAlgorithm?: HashAlgorithmNameOrOid;
  @Input() set hashAlgorithm(value: HashAlgorithmNameOrOid | undefined) {
    this._hashAlgorithm = value;
  }

  @Input() hint?: string;

  readonly optionsHash: HashAlgorithmNameOrOid[] = Object.keys(HashAlgorithmNames);

  @Output() hashAlgorithmChange = new EventEmitter<HashAlgorithmNameOrOid | undefined>();

  get alg() {
    return this._hashAlgorithm;
  }

  set alg(value) {
    if(value == null || value.trim() == "") {
      this._hashAlgorithm = undefined;
    } else {
      this._hashAlgorithm = value;
    }
    this.hashAlgorithmChange.emit(this._hashAlgorithm);
  }
}
