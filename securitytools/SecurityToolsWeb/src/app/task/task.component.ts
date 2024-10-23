import { Component, OnDestroy, OnInit } from '@angular/core';
import { WorkerService } from './services/worker.service';
import { interval, Subject, takeUntil } from 'rxjs';
import { Dictionary } from '../tools/services/models';
import { WorkState, WorkStatus, WorkEventArgs } from './services/models';

@Component({
  selector: 'app-task',
  templateUrl: './task.component.html',
  styleUrl: './task.component.css'
})
export class TaskComponent implements OnInit, OnDestroy {
  public WorkState = WorkState;

  private unsubscribe = new Subject<void>();

  status: Dictionary<string, WorkStatus> = {};
  events: WorkEventArgs[] = [];

  requestId = "";

  limit = 25;

  constructor(private workerService: WorkerService) {}

  ngOnInit(): void {
    interval(1000).pipe(takeUntil(this.unsubscribe)).subscribe({
      next: () => {
        this.workerService.getEvents().pipe(takeUntil(this.unsubscribe)).subscribe({
          next: (events) => { this.events = events; }
        });
      }
    });
    this.refresh();
  }

  ngOnDestroy(): void {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

  refresh() {
    this.workerService.getStatus(this.limit).pipe(takeUntil(this.unsubscribe)).subscribe({
      next: (status) => { this.status = status; }
    });
  }

  cancel() {
    this.workerService.cancel(this.requestId).subscribe({
      next: () => { this.refresh(); },
      error: () => { this.refresh(); }
    });
    this.requestId = "";
  }

  getKeys() {
    return Object.keys(this.status);
  }

  getValue(id: string) {
    return this.status[id];
  }
}
