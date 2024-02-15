import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HashAlgorithmComponent } from './hash-algorithm.component';

describe('HashAlgorithmComponent', () => {
  let component: HashAlgorithmComponent;
  let fixture: ComponentFixture<HashAlgorithmComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ HashAlgorithmComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HashAlgorithmComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
