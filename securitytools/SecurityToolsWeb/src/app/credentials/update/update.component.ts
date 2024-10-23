import { Component, OnDestroy, OnInit } from '@angular/core';
import { KeyCredentials } from '../services/models';
import { CredentialsService } from '../services/credentials.service';
import { ActivatedRoute, Router } from '@angular/router';
import { mergeMap, Subject, switchMap } from 'rxjs';

@Component({
  selector: 'app-update',
  templateUrl: './update.component.html',
  styleUrl: './update.component.css'
})
export class UpdateComponent implements OnInit, OnDestroy {
  private initial?: KeyCredentials;
  item?: KeyCredentials;
  id?: string;

  private unsubscribe = new Subject<void>();

  constructor(private service: CredentialsService, private route: ActivatedRoute, private router: Router) { }

  ngOnInit(): void {
    this.route.params.subscribe({
      next: (params) => {
        // refresh to load selected connection
        this.id = params['id'];
        this.service.getCredentialsById(this.id!).subscribe({
          next:(value) => {
            this.initial = value;
            this.item = JSON.parse(JSON.stringify(value));
          }
        })
      }
    });
  }

  ngOnDestroy(): void {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

  cancel() {
    this.router.navigateByUrl("credentials");
  }

  get hasChange() {
    return JSON.stringify(this.item) != JSON.stringify(this.initial);
  }

  update() {
    if(this.id == null || this.item == null) {
      return;
    }
    this.service.updateCredentials(this.id, this.item).subscribe({
      next: () => {
        this.cancel();
      }
    })
  }
}
