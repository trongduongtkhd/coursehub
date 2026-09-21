export interface User {
  id: number;
  fullName: string;
  email: string;
  role: 'Admin' | 'Instructor' | 'Student';
}
