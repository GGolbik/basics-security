import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Subscription, map, timer } from 'rxjs';
import { LogConfig, LogEntry, LogFilterOptions, LogLevel } from '../services/models';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { toLocalISOString } from 'src/main';
import { LoggingService } from '../services/log.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-log',
  templateUrl: './log.component.html',
  styleUrls: ['./log.component.css']
})
export class LogComponent implements OnInit, OnDestroy, AfterViewInit {
  public Number = Number;
  public Loglevel = LogLevel;

  dataSource: MatTableDataSource<LogEntry> = new MatTableDataSource<LogEntry>([]);

  filter: LogFilterOptions = {
    level: LogLevel.Information,
    limit: 100
  };

  @ViewChild(MatPaginator) paginator?: MatPaginator;

  pageSizeOptions: number[] = [5, 10, 25, 50, 75, 100];
  dataSourceLength: number = 0;
  currentPage: number = 0;

  toLocalDateTime = true;
  recording: boolean = true;
  recordingInterval: number = 1000;

  config = { level: LogLevel.None } as LogConfig;
  
  get columns() {
    let result = [];
    
    result.push({
      columnDef: 'Timestamp',
      header: 'Timestamp',
      cell: (element: LogEntry) => `${this.toLocalDateTime ? toLocalISOString(new Date(element.timestamp ?? "")) : element.timestamp}`,
    });
  
  
    result.push({
      columnDef: 'Level',
      header: 'Level',
      cell: (element: LogEntry) => `${LogLevel[Number(element.level)]}`,
    });

    result.push({
      columnDef: 'Message',
      header: 'Message',
      cell: (element: LogEntry) => `${element.message}`,
    });
    
    return result;
  }
  get displayedColumns() {
    return this.columns.map(c => c.columnDef);
  }

  private resetPager() {
    this.dataSourceLength = 0;
    this.currentPage = 0;
  }
  constructor(private loggingService: LoggingService, private snackBar: MatSnackBar) {
  }

  getStyle(row: any) {
    if(row as LogEntry){
      //console.log(row.level);
    }
    return "backround: orange !important";
  }
  
  getTag(entry: LogEntry) {
    return JSON.parse(entry.properties ?? "{}")["SourceContext"] ?? "";
  }

  updateData() {
    this.dataSource.data = [];
    this.dataSourceLength = 0;
    if (this.recording) {
      this.startTimer();
    } else {
      let filter = this.filter;
      filter.offset = 0;
      this.loadData(filter, this.recording);
    }
  }

  private onPageEvent(event: PageEvent) {
    // if recording, only update options
    if (this.filter.limit != event.pageSize) {
      this.resetPager();
    } else {
      this.currentPage = event.pageIndex;
    }
    this.filter.limit = event.pageSize;
    if (this.recording) {
      return;
    }
    this.refresh(false);
  };

  /**
   * Starts the timer if not running yet.
   */
  startTimer(): void {
    if (!this.recording || this.timerSubscription != null) {
      return;
    }
    // update all 2 seconds
    this.timerSubscription = timer(0, this.recordingInterval).pipe(
      map(() => {
        this.loadData(this.filter, this.recording);
      })
    ).subscribe();
  }

  
  toggleRecording() {
    this.stopTimer();
    this.recording = !this.recording;
    if (this.recording) {
      this.dataSource.data = [] as LogEntry[];
      this.resetPager();
     
      this.startTimer();
    } else {
      this.refresh(true);
    }
  }

  ngOnInit(): void {
    this.loadConfig();
    this.startTimer()
  }

  clear() {
    this.loggingService.deleteEntries().subscribe({
      next: (value) => {
        this.dataSource.data = [] as LogEntry[];
      }
    });
  }

  loadConfig() {
    this.loggingService.getConfig().subscribe({
      next: (config) => {
        this.config = config;
      },
      error: (err) => {
        console.error(err);
      }
    });
  }

  refreshDate?: string = new Date().toISOString();
  refresh(update: boolean) {
    if (update) {
      // use current date as anchor or the last date of record if the record timestamp is newer
      let newDate = new Date();
      if (this.dataSource.data.length > 0 && this.dataSource.data[0].timestamp != null) {
        let lastDate = new Date(this.dataSource.data[0].timestamp);
        if (lastDate > newDate) {
          this.refreshDate = this.dataSource.data[0].timestamp;
        } else {
          this.refreshDate = newDate.toISOString();
        }
      }
    }
    // if not recording, load the data
    let filter = JSON.parse(JSON.stringify(this.filter)); // copy options
    // set offset
    filter.offset = this.currentPage * filter.limit!;
    if (this.refreshDate != null && filter.toClientTimestamp == null) {
      filter.toClientTimestamp = this.refreshDate;
    }
    // load data
    this.loadData(filter, this.recording);
  }
  ngOnDestroy(): void {
    this.stopTimer();
  }

  
  /**
   * Sets the value for autotrust
   */
  setConfig() {
    this.loggingService.setConfig(this.config).subscribe({
      next: (result) => {
        this.loadConfig();
      },
      error: (err) => {
        console.error(err);
      }
    });
  }

  private loadData(filter: LogFilterOptions, recording: boolean) {
    // if not recording we have to diable buttons
    this.loggingService.getEntries(filter).subscribe({
      next: (result) => {
        // update data source
        this.dataSource.data = result;
        // set length
        let offset = filter.offset ?? 0;
        let limit = filter.limit ?? 10;
        if (recording) {
          this.dataSourceLength = result.length;
        } else {
          this.dataSourceLength = offset + (result.length >= limit! ? limit + 1 : result.length);
        }
      },
      error: (err) => {
        console.error(err)
      }
    });
  }


  
  openContent(value: any) {
    /*
    let data = value as ClientDataValue;
    const dialogRef = this.dialog.open(DataViewDialogComponent);
    dialogRef.componentInstance.data = data;
    */
  }
  ngAfterViewInit(): void {
    if (this.paginator != null) {
      this.paginator.page.subscribe(event => {
        this.onPageEvent(event);
      });
    }
  }
  /**
   * Timer to refresh connection states.
   */
  private timerSubscription: Subscription | undefined;
  stopTimer(): void {
    // stop timer
    this.timerSubscription?.unsubscribe();
    this.timerSubscription = undefined;
  }
}