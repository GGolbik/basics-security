import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { CredentialsService } from '../services/credentials.service';
import { Subject, takeUntil } from 'rxjs';
import { Dictionary, KeyCredentials, KeyCredentialsKind } from '../services/models';
import { MatExpansionPanel } from '@angular/material/expansion';
import { Router } from '@angular/router';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrl: './list.component.css'
})
export class ListComponent implements OnInit, OnDestroy {
  public KeyCredentialsKind = KeyCredentialsKind;

  credentials: Dictionary<string, KeyCredentials> = {};

  password?: string;

  item?: KeyCredentials;

  @ViewChild('panel') panel?: MatExpansionPanel;

  private unsubscribe = new Subject<void>();

  constructor(private service: CredentialsService, private router: Router) {

  }

  ngOnInit(): void {
    this.refresh();
  }

  ngOnDestroy(): void {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

  private refresh() {
    this.service.getCredentials().pipe(takeUntil(this.unsubscribe)).subscribe({
      next: (credentials) => {
        this.credentials = credentials;
      }
    })
  }

  getKeys() {
    return Object.keys(this.credentials);
  }

  open(id: string) {
    this.router.navigateByUrl(`credentials/list/${id}`);
  }

  reset() {
    this.item = undefined;
  }

  add() {
    if (this.item == null) {
      return;
    }
    this.service.addCredentials(this.item, this.password).pipe(takeUntil(this.unsubscribe)).subscribe({
      next: (id) => {
        this.panel?.close();
        this.reset();
        this.refresh();
      },
      error: (err) => {
        console.error(err);
      }
    });
  }

  remove(id: string) {
    this.service.deleteCredentials(id).pipe(takeUntil(this.unsubscribe)).subscribe({
      next: () => {
        this.refresh();
      },
      error: (err) => {
        console.error(err);
      }
    });
  }
}
