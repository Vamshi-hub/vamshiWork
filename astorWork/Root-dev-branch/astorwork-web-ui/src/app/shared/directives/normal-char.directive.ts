import { ValidatorFn, AbstractControl } from "@angular/forms";

export function normalCharacterValidator(control: AbstractControl) {
    const regEx = /^[a-zA-Z]*$/;
    const normalChar = regEx.test(control.value);
    console.log("normalChar? ", normalChar);
    return normalChar ?  null : { 'normalChar': 'Special characters are not allowed' };
}