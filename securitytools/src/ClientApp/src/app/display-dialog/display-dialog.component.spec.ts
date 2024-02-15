import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplayDialogComponent } from './display-dialog.component';

describe('DisplayDialogComponent', () => {
  let component: DisplayDialogComponent;
  let fixture: ComponentFixture<DisplayDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisplayDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DisplayDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
