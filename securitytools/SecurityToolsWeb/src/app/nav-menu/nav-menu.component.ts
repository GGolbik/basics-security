import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit, OnDestroy {

  isExpanded = false;
  index = 0;

  private tools = [
    '/tools/keypair',
    '/tools/csr',
    '/tools/certificate',
    '/tools/selfsigncertificate',
    '/tools/crl',
    '/tools/transform'
  ];

  private unsubscribe = new Subject<void>();

  constructor(private router: Router) {}

  ngOnInit(): void {
    this.router.events.pipe(takeUntil(this.unsubscribe)).subscribe({
      next:() => {
        this.index = this.tools.findIndex((item) => {
          return this.router.url.startsWith(item);
        })
      }
    })
  }
  ngOnDestroy(): void {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  isTools() {
    return this.router.url.startsWith("/tools");
  }

  nav(index: number) {
    let url = this.tools.at(index);
    if(url)
      this.router.navigateByUrl(url);
  }
}
