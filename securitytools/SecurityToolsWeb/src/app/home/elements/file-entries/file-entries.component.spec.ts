import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FileEntriesComponent } from './file-entries.component';

describe('FileEntriesComponent', () => {
  let component: FileEntriesComponent;
  let fixture: ComponentFixture<FileEntriesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FileEntriesComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FileEntriesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
