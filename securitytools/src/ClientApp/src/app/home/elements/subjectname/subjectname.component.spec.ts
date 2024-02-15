import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubjectnameComponent } from './subjectname.component';

describe('SubjectnameComponent', () => {
  let component: SubjectnameComponent;
  let fixture: ComponentFixture<SubjectnameComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SubjectnameComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SubjectnameComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
